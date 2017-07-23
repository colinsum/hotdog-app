using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Plugin.Geolocator;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Tabs
{
	public partial class AzureTables : ContentPage
	{

		public AzureTables()
		{
			InitializeComponent();

		}

		async void Handle_ClickedAsync(object sender, System.EventArgs e)

		{
            loading.IsVisible = true;
            HotDogList.ItemsSource = "";
            List<csumcarapptable> csumcarapp = await AzureManager.AzureManagerInstance.GetInformation();
            string tag = AzureManager.AzureManagerInstance.getTag();
            loading.IsVisible = false;

            if (tag != "Car not found")
            {
                HotDogList.ItemsSource = csumcarapp.Where(x => x.model == tag);
            }

            //HotDogList.ItemsSource = csumcarapp;
		}	
    }
}
