﻿using Butterfly.HabboHotel.Roleplay.Item;

namespace Butterfly.HabboHotel.Roleplay
{
    public class RPItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; }
        public string Desc { get; set; }
        public int Price { get; set; }
        public string Type { get; set; }
        public int Value { get; set; }
        public bool AllowStack { get; set; }
        public RPItemCategory Category { get; set; }
        public int UseType { get; }

        public RPItem(int pId, string pName, string pDesc, int pPrice, string pType, int pValue, bool pAllowStack, RPItemCategory pCatagory)
        {
            this.Id = pId;
            this.Name = pName;
            this.Title = pDesc;
            this.Desc = _generateDesc(pDesc, pType, pValue);
            this.Price = pPrice;
            this.Type = pType;
            this.Value = pValue;
            this.AllowStack = pAllowStack;
            this.Category = pCatagory;
            this.UseType = _getUseType(Type);
        }

        private int _getUseType(string Type)
        {
            /*  UseType:
                0: Non utilisable
                1: Utilisable mais limité à 1
                2: Utilisable et possibilité de choisir le nombre à utiliser
            */
            switch (Type)
            {
                case "openguide":
                    {
                        return 1;
                    }
                case "hit":
                    {
                        return 2;
                    }
                case "enable":
                    {
                        return 1;
                    }
                case "showtime":
                    {
                        return 1;
                    }
                case "money":
                    {
                        return 2;
                    }
                case "munition":
                    {
                        return 2;
                    }
                case "energytired":
                    {
                        return 2;
                    }
                case "healthtired":
                    {
                        return 2;
                    }
                case "healthenergy":
                    {
                        return 2;
                    }
                case "energy":
                    {
                        return 2;
                    }
                case "health":
                    {
                        return 2;
                    }
                case "weapon_cac":
                    {
                        return 1;
                    }
                case "weapon_far":
                    {
                        return 1;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        private string _generateDesc(string Desc, string Type, int Value)
        {
            string Text = "<u>" + Desc + "</u></br />";

            switch (Type)
            {
                case "openguide":
                    {
                        Text += "Divers aide pour commencer le jeu";
                        break;
                    }
                case "hit":
                    {
                        Text += "Vous fait perdre " + Value + " vie";
                        break;
                    }
                case "enable":
                    {
                        Text += "Active l'enable " + Value;
                        break;
                    }
                case "showtime":
                    {
                        Text += "Affiche l'heure du jeu";
                        break;
                    }
                case "money":
                    {
                        Text += "Ajoute " + Value + " Dollars";
                        break;
                    }
                case "munition":
                    {
                        Text += "Ajoute " + Value + " munitions";
                        break;
                    }
                case "energytired":
                    {
                        Text += "Augmente votre énergie de " + Value + " mais diminue votre vie du même montant";
                        break;
                    }
                case "healthtired":
                    {
                        Text += "Augmente votre vie de " + Value + " mais diminue votre énergie du même montant";
                        break;
                    }
                case "healthenergy":
                    {
                        Text += "Augmente votre vie et énergie de " + Value;
                        break;
                    }
                case "energy":
                    {
                        Text += "Augmente votre énergie de " + Value;
                        break;
                    }
                case "health":
                    {
                        Text += "Soigne " + Value + " points de vie";
                        break;
                    }
                case "weapon_cac":
                    {
                        Text += "Arme corps à corps (:cac x)";
                        break;
                    }
                case "weapon_far":
                    {
                        Text += "Arme distante (:pan)";
                        break;
                    }
                default:
                    {
                        Text += "Non utilisable";
                        break;
                    }
            }

            return Text;
        }
    }
}
