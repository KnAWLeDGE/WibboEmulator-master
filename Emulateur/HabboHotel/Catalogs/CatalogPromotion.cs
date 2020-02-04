﻿using Butterfly.Core;

namespace Butterfly.HabboHotel.Catalog
{
    public class CatalogPromotion
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleEn { get; set; }
        public string TitleBr { get; set; }
        public string Image { get; set; }
        public int Unknown { get; set; }
        public string PageLink { get; set; }
        public int ParentId { get; set; }

        public CatalogPromotion(int id, string title, string titleEn, string titleBr, string image, int unknown, string pageLink, int parentId)
        {
            this.Id = id;
            this.Title = title;
            this.TitleEn = titleEn;
            this.TitleBr = titleBr;
            this.Image = image;
            this.Unknown = unknown;
            this.PageLink = pageLink;
            this.ParentId = parentId;
        }

        public string GetTitleByLangue(Language Langue)
        {
            if (Langue == Language.ANGLAIS)
                return TitleEn;
            else if (Langue == Language.PORTUGAIS)
                return TitleBr;

            return Title;
        }
    }
}
