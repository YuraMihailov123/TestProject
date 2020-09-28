using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Linq;
using System.Text;

namespace TestProject
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        static readonly HttpClient client = new HttpClient();

        ListView mainList;

        List<string> mainData;

        string url = "http://partner.market.yandex.ru/pages/help/YML.xml";
        string response = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            mainList = FindViewById<ListView>(Resource.Id.listView1);
            mainData = new List<string>();
            GetInfo();
        }

        public async void GetInfo()
        {
            response = await GetInfoAsync(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);
            var text = doc.DocumentNode.SelectNodes("//offers//offer");//.Select(node => node.InnerHtml);//node.GetAttributes().ToString());

            Console.WriteLine(text.Count);
            List<string> tempList = new List<string>();
            for (int i=0;i< text.Count;i++)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Id: " + text[i].Id);
                foreach (var node in text[i].Descendants())
                { 
                    if (node.OriginalName != "#text")
                    {
                        //Console.WriteLine(node.OriginalName + ":" + node.InnerText);
                        stringBuilder.AppendLine(node.OriginalName + ": " + node.InnerText);
                    }
                    
                }
                tempList.Add(stringBuilder.ToString());
                Console.WriteLine(i);
                Console.WriteLine(stringBuilder);

                //Console.WriteLine(text[i].SelectSingleNode("/yml_catalog/shop/offers/offer[" + (i+1)+"]/price").InnerText);
                //Console.WriteLine(text[i].Id);
                //mainData.Add(line);
            }
            mainData = new List<string>(tempList);
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, GetOnlyIDs(mainData));
            mainList.Adapter = adapter;
            mainList.ItemClick += MainList_ItemClick;
        }

        public List<string> GetOnlyIDs(List<string> list)
        {
            List<string> returnList = new List<string>();
            for(int i = 0; i < list.Count; i++)
            {
                returnList.Add(list[i].Split("\n")[0].Split(" ")[1]);
            }
            return returnList;
        }

        //public void GetInnerText(string)

        private void MainList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var NxtAct = new Intent(this, typeof(ResultActivity));
            NxtAct.PutExtra("content", mainData[e.Position]);
            StartActivity(NxtAct);
        }

        async Task<string> GetInfoAsync(string _url)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                HttpResponseMessage response = await client.GetAsync(_url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                Console.WriteLine(responseBody);
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return "";
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
