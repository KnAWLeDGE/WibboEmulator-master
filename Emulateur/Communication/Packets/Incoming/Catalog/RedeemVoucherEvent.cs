using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Catalog.Vouchers;
using System.Data;
using Butterfly.Database.Interfaces;
using Butterfly.Communication.Packets.Outgoing.Structure;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class RedeemVoucherEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string VoucherCode = Packet.PopString().Replace("\r", "");

            Voucher Voucher = null;
            if (!ButterflyEnvironment.GetGame().GetCatalog().GetVoucherManager().TryGetVoucher(VoucherCode, out Voucher))
            {
                Session.SendPacket(new VoucherRedeemErrorComposer(0));
                return;
            }

            if (Voucher.CurrentUses >= Voucher.MaxUses)
                return;

            DataRow GetRow = null;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_vouchers` WHERE `user_id` = @userId AND `voucher` = @Voucher LIMIT 1");
                dbClient.AddParameter("userId", Session.GetHabbo().Id);
                dbClient.AddParameter("Voucher", VoucherCode);
                GetRow = dbClient.GetRow();
            }

            if (GetRow != null)
                return;
            else
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_vouchers` (`user_id`,`voucher`) VALUES (@userId, @Voucher)");
                    dbClient.AddParameter("userId", Session.GetHabbo().Id);
                    dbClient.AddParameter("Voucher", VoucherCode);
                    dbClient.RunQuery();
                }
            }

            Voucher.UpdateUses();

            if (Voucher.Type == VoucherType.CREDIT)
            {
                Session.GetHabbo().Credits += Voucher.Value;
                Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }
            else if (Voucher.Type == VoucherType.DUCKET)
            {
                Session.GetHabbo().Duckets += Voucher.Value;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Voucher.Value));
            }

            //Session.SendPacket(new VoucherRedeemOkComposer());
        }
    }
}
