using PKHeX.Core;
using System;
using System.Threading.Tasks;
using Discord;
using System.Net.Http;
using SysBot.Base;
using System.Net;

namespace SysBot.Pokemon
{
    public class Roshicl
    {
        static readonly HttpClient client = new();
        public async Task<string> EmbedImgUrlBuilder(PKM mon, bool canGMax, string URLFormArg)
        {
            string URLStart = "https://raw.githubusercontent.com/PhantomL98/HomeImages/main/Sprites/200x200/poke_capture";
            string URLString, URLGender;
            string URLGMax = canGMax ? "g" : "n";
            string URLShiny = mon.IsShiny ? "r.png" : "n.png";

            if (mon.Gender < 2)
                URLGender = "mf";
            else
                URLGender = "uk";

            URLString = URLStart + "_" + mon.Species.ToString("0000") + "_" + mon.Form.ToString("000") + "_" + URLGender + "_" + URLGMax + "_" + URLFormArg + "_f_" + URLShiny;

            if (mon.IsEgg)
                URLString = "https://raw.githubusercontent.com/PhantomL98/HomeImages/main/Sprites/512x512/egg.png";

            try
            {
                using HttpResponseMessage response = await client.GetAsync(URLString);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                if (mon.Gender == 0)
                    URLGender = "md";
                else
                    URLGender = "fd";

                URLString = URLStart + "_" + mon.Species.ToString("0000") + "_" + mon.Form.ToString("000") + "_" + URLGender + "_" + URLGMax + "_" + URLFormArg + "_f_" + URLShiny;

                try
                {
                    using HttpResponseMessage response = await client.GetAsync(URLString);
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex1) when (ex1.StatusCode == HttpStatusCode.NotFound)
                {
                    if (mon.Gender == 0)
                        URLGender = "mo";
                    else
                        URLGender = "fo";

                    URLString = URLStart + "_" + mon.Species.ToString("0000") + "_" + mon.Form.ToString("000") + "_" + URLGender + "_" + URLGMax + "_" + URLFormArg + "_f_" + URLShiny;
                }
            }
            return URLString;
        }
        public static string FixHeldItemName(string name)
        {
            name = name.Replace("____", " ");
            name = name.Replace("___", ".");
            name = name.Replace("__", "'");
            name = name.Replace("_", "-");
            return name;
        }
    }
}