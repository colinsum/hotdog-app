using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Tabs.Model;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.Geolocator;
using System.Collections.Generic;

namespace Tabs
{
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            TagLabel.Text = "";

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });


            await MakePredictionRequest(file);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "212601d8ea68473e95e11d756d23a745");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/2ff50d41-5b0a-419c-b01c-74978fb61f1e/image?iterationId=80f206ff-002d-4cb3-b9dc-452e71c17c37";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                    double max = responseModel.Predictions.Max(m => m.Probability);

                    List<Prediction> predictions = responseModel.Predictions;
                    string tag = "";

                    foreach (Prediction p in predictions)
                    {
                        if (p.Probability == max)
                        {
                            tag = p.Tag;
                        }
                    }



                    if (max >= 0.5) {
                        
                        TagLabel.Text = tag;
                        AzureManager.AzureManagerInstance.setTag(tag);

                    }
                    else {
                        tag = "Car not found";
                        AzureManager.AzureManagerInstance.setTag(tag);
                        TagLabel.Text = tag;
                    }


                }

                //Get rid of file once we have finished using it
                file.Dispose();
            }
        }
	}
}