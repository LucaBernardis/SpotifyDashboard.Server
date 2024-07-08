﻿using SpotifyDashboard.Server.Models;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace SpotifyDashboard.Server.Services
{
    public partial class DashboardService
    {
        /// <summary>
        /// Method to get the data of the user's favourite artist trough
        /// an http request to the dedicated spotify api address
        /// </summary>
        /// <returns> An Artist object with the usefull data about the favourite artist of the user </returns>
        public async Task<Artist> GetTopArtist()
        {
            // Http call to the spotify api address
            using HttpResponseMessage response = await _httpClient.GetAsync("v1/me/top/artists?limit=1");
            response.EnsureSuccessStatusCode(); // Throw an exception if the response is not successful

            var responseBody = await response.Content.ReadAsStringAsync();
            var jObj = JsonNode.Parse(responseBody)?.AsObject();
            var items = jObj?["items"]?.AsArray();

            var artistJson = items[0]?.ToJsonString();
            var artist = JsonSerializer.Deserialize<Artist>(artistJson);

            // Assign to the Genres property the first value of the genres array
            var genres = items[0]["genres"]?.AsArray();
            artist.Genres = genres[0]?.ToString();

            // Assign to the ImageUrl property the value of the image array url
            if (artist != null)
            {
                var images = jObj["items"][0]["images"]?.AsArray();
                artist.ImageUrl = images.FirstOrDefault()?["url"]?.ToString();
            }

            return artist;
        }

        /// <summary>
        /// Method to get the most famous track of the user's favourite artist
        /// </summary>
        /// <param name="artistId"> The id of the artist you need to pass in the api call to get its related most famouse song </param>
        /// <returns> A Track object with the artist's most famous track data </returns>
        public async Task<Track> GetArtistTopTrack(string artistId)
        {
            // Http call to the spotify api address
            using HttpResponseMessage response = await _httpClient.GetAsync($"/v1/artists/{artistId}/top-tracks");

            response.EnsureSuccessStatusCode(); // Throw an exception if the response is not successful

            var responseBody = await response.Content.ReadAsStringAsync();

            var jObj = JsonNode.Parse(responseBody)?.AsObject();
            var tracks = jObj?["tracks"]?.AsArray();

            var track = JsonSerializer.Deserialize<List<Track>>(tracks.ToJsonString());

            // Assign to the ImageUrl property the value of the album image url
            var album = tracks[0]["album"]?.AsObject();
            var albumImg = album?["images"]?.AsArray();
            track[0].ImageUrl = albumImg[0]["url"]?.ToString();

            // Assign to the Name property the value of the first track name
            track[0].Name = tracks[0]["name"]?.ToString();

            // Assign to the artist property the value of the first artist name
            var artist = tracks[0]["artists"]?.AsArray();
            track[0].Artist = artist[0]["name"]?.ToString();

            var topTrack = track[0];
            return topTrack;
        }

        /// <summary>
        /// Method to get all the albums the artist made or is part of
        /// </summary>
        /// <param name="artistId"> The id of the artist you need to pass in the api call to get all its related albums </param>
        /// <returns> A list of the albums the artist made or is part of and the related usefull data </returns>
        public async Task<IEnumerable<Album>> GetAlbums(string artistId)
        {
            // Http call to the spotify api address
            using HttpResponseMessage response = await _httpClient.GetAsync($"/v1/artists/{artistId}/albums");

            response.EnsureSuccessStatusCode(); // Throw an exception if the response is not successful

            var responseBody = await response.Content.ReadAsStringAsync();

            var jObj = JsonNode.Parse(responseBody)?.AsObject();
            var albums = jObj?["items"]?.AsArray();


            var albumList = JsonSerializer.Deserialize<List<Album>>(albums.ToJsonString());

            // For Each album object
            for (int i = 0; i < albumList.Count; i++)
            {
                var album = albumList[i];
                var item = albums[i];

                // Assign to the SpotifyUrl property the value of the spotify external url
                var extUrl = item?["external_urls"]?.AsObject();
                album.SpotifyUrl = extUrl["spotify"]?.ToString();

                // Assign to the ImaegeUrl the value of the image array url
                var image = item?["images"]?.AsArray();
                var imageUrl = image[0]["url"]?.ToString();
                album.ImageUrl = imageUrl;

                // Assign to the Artist property the value of the artist name
                var artist = item?["artists"]?.AsArray();
                var artistName = artist[0]["name"]?.ToString();
                album.Artist = artistName;
            }

            return albumList;

        }

        /// <summary>
        /// Method to retrieve spotify new releases, its a list of albums with all the related data, 
        /// like total tracks, data about the artist and external links to the spotify page
        /// </summary>
        /// <returns> A list of album with name, image url and number of tracks </returns>
        public async Task<IEnumerable<Album>> GetNewReleases()
        {
            //// Http call to the spotify api address
            using HttpResponseMessage response = await _httpClient.GetAsync($"/v1/browse/new-releases");

            response.EnsureSuccessStatusCode(); // Throw an exception if the response is not successful

            var responseBody = await response.Content.ReadAsStringAsync();

            var jObj = JsonNode.Parse(responseBody)?.AsObject();
            var wrapper = jObj?["albums"]?.AsObject();
            var releases = wrapper?["items"]?.AsArray();

            var newReleases = JsonSerializer.Deserialize<List<Album>>(releases.ToJsonString());

            for (int i = 0; i < newReleases.Count; i++)
            {
                var newRelease = newReleases[i];
                var item = releases[i];

                // Assign to the SpotifyUrl property the value of the spotify external url
                var extUrl = item?["external_urls"]?.AsObject();
                newRelease.SpotifyUrl = extUrl["spotify"]?.ToString();

                // Assign to the ImaegeUrl the value of the image array url
                var image = item?["images"]?.AsArray();
                var imageUrl = image[0]["url"]?.ToString();
                newRelease.ImageUrl = imageUrl;

                // Assign to the Artist property the value of the artist name
                var artist = item?["artists"]?.AsArray();
                var artistName = artist[0]["name"]?.ToString();
                newRelease.Artist = artistName;
            }

            return newReleases;

        }

    }
}
