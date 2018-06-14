using System;
using System.Collections.Generic;
using HttpTask;

namespace HttpClient
{
    public class SiteLoaderConsole
    {
        static void Main(string[] args)
        {
            SiteLoader siteLoader = new SiteLoader(new List<ILimitation>(),5, Logger.Instance, new SiteSaver("D:\\New folder (4)", Logger.Instance) );

            siteLoader.LoadFromUrl("https://stackoverflow.com/questions/8472678/is-it-a-good-practice-to-have-logger-as-a-singleton");

            Console.Read();
        }
    }
}
