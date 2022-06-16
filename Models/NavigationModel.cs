using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BiblePay.BMS.Extensions;
using Microsoft.AspNetCore.Http;

namespace BiblePay.BMS.Models
{
    public static class NavigationModel
    {
        private const string Underscore = "_";
        private const string Dash = "-";
        private const string Space = " ";
        private static readonly string Empty = string.Empty;
        public static readonly string Void = "javascript:void(0);";

        public static SmartNavigation Seed => BuildNavigation();
        public static SmartNavigation Full => BuildNavigation(seedOnly: false);

        private static SmartNavigation BuildNavigation(bool seedOnly = true)
        {
            var jsonText = File.ReadAllText("nav.json");
            var navigation = NavigationBuilder.FromJson(jsonText);
            var menu = FillProperties(navigation.Lists, seedOnly);
            return new SmartNavigation(menu);
        }

        private static List<ListItem> FillProperties(IEnumerable<ListItem> items, bool seedOnly, ListItem parent = null)
        {
            var result = new List<ListItem>();
            double nCoreBalance = BMSCommon.WebRPC.GetCachedCoreWalletBalance(false);

            //bool fLogged = DSQL.UI.GetUser(HttpContext).LoggedIn;
        
            foreach (var item in items)
            {
                item.Text ??= item.Title;
                item.Tags = string.Concat(parent?.Tags, Space, item.Title.ToLower()).Trim();

                var parentRoute = (Path.GetFileNameWithoutExtension(parent?.Text ?? Empty)?.Replace(Space, Underscore) ?? Empty).ToLower();
                var sanitizedHref = parent == null ? item.Href?.Replace(Dash, Empty) : item.Href?.Replace(parentRoute, parentRoute.Replace(Underscore, Empty)).Replace(Dash, Empty);
                var route = Path.GetFileNameWithoutExtension(sanitizedHref ?? Empty)?.Split(Underscore) ?? Array.Empty<string>();
                bool fDisabled = false;
                item.Route = route.Length > 1 ? $"/{route.First()}/{string.Join(Empty, route.Skip(1))}" : item.Href;
                if (nCoreBalance < 1000000)
                {
                    if (item.Route != null)
                    {
                        if (item.Route.ToLower().Contains("nft") || item.Route == "/bbp/proposaladd" || item.Route == "/bbp/proposallist" || item.Route == "/bbp/turnkeysanctuaries" || item.Route == "/bbp/portfoliobuilder" || item.Route == "/bbp/portfoliobuilderleaderboard" || item.Route == "/bbp/portfoliobuilderdonation")
                        {
                            fDisabled = true;
                        }
                    }
                }

                item.I18n = parent == null
                    ? $"nav.{item.Title.ToLower().Replace(Space, Underscore)}"
                    : $"{parent.I18n}_{item.Title.ToLower().Replace(Space, Underscore)}";
                item.Type = parent == null ? item.Href == null ? ItemType.Category : ItemType.Single : item.Items.Any() ? ItemType.Parent : ItemType.Child;
                item.Items = FillProperties(item.Items, seedOnly, item);

                if (item.Href.IsVoid() && item.Items.Any())
                    item.Type = ItemType.Sibling;

                if (!seedOnly || item.ShowOnSeed)
                {
                    if (!fDisabled)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }
    }
}
