
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TestProject
{
    [Activity(Label = "ResultActivity")]
    public class ResultActivity : Activity
    {
        TextView contentText;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_result);
            contentText = FindViewById<TextView>(Resource.Id.textView1);
            var content = Intent.GetStringExtra("content") ?? "-";
            contentText.Text = content;
            ConvertToJson(content);
            Console.WriteLine(content);
        }

        private void ConvertToJson(string text)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.AppendLine("{");
            jsonString.AppendLine("\t\"offer\":\t{");
            foreach (var str in text.Split("\n"))
            {
                if (str != "")
                {
                    jsonString.AppendLine("\t\t\"" + str.Split(" ")[0].Replace(":","") + "\": " + str.Split(" ")[1]+",");
                }
            }
            jsonString.AppendLine("\t}");
            jsonString.AppendLine("}");


            contentText.Text = jsonString.ToString();
        }
    }
}
