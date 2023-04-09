//using Google.Apis.Services;
//using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiblePay.BMS.DSQL
{
    public static class youtube
    {
        public static string GetVideo(HttpContext h)
        {
            string sID = h.Request.Query["id"];
            string url = "<iframe width='1000' height='600' src='https://www.youtube.com/embed/" + sID
                + "?rel=0' title='YouTube video player' frameborder='0' allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture' allowfullscreen></iframe>";
            string div = "<div>" + url + "</div>";
            return div;
        }
        public static string GetSomeVideos(HttpContext h)
        {
            /*
            string search = h.Request.Query["search"];
            if (search==null || search == "")
            {
                search = "Christian Videos";
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBLFvsyCKwatVTAGqGMxcMJAGibD2fC3rk",
                ApplicationName = "bbp"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = search;
            searchListRequest.MaxResults = 33;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();
            List<string> channels = new List<string>();
            List<string> playlists = new List<string>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            string html = "";
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                        
                        string url = "<iframe width='333' height='255' src='https://www.youtube.com/embed/" + searchResult.Id.VideoId.ToString() + "' title='YouTube video player' frameborder='0' allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture' allowfullscreen></iframe>";
                        string a = "<a href='bbp/watch?id=" + searchResult.Id.VideoId.ToString() + "'>";

                        url = a + "<img src='https://img.youtube.com/vi/" + searchResult.Id.VideoId.ToString() + "/hqdefault.jpg' width='333' height='255'/></a>";
                        url += "<br><div style='width:333px;overflow-y:auto;'>" + searchResult.Snippet.Title + "<br>" + searchResult.Snippet.ChannelTitle + "</div>";

                        string div = "<div>" + url + "</div><br><br>";

                        html += div;
                        break;

                    case "youtube#channel":
                        channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                        break;
                }
            }

            return html;
            */
            return String.Empty;
        }
    }

}
