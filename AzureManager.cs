using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Tabs
{
	public class AzureManager
	{

		private static AzureManager instance;
		private MobileServiceClient client;
	    private IMobileServiceTable<csumcarapptable> infoTable;
        private string tag;


	private AzureManager()
		{
			this.client = new MobileServiceClient("http://csumcarapp.azurewebsites.net");
            this.infoTable = this.client.GetTable<csumcarapptable>();
		}

		public MobileServiceClient AzureClient
		{
			get { return client; }
		}


		public static AzureManager AzureManagerInstance
		{
			get
			{
				if (instance == null)
				{
					instance = new AzureManager();
				}

				return instance;
			}
		}
        public async Task<List<csumcarapptable>> GetInformation()

        {
           return await this.infoTable.ToListAsync();
		}



        public string getTag() {
            return this.tag;
        }

        public void setTag(string tag) {
            this.tag = tag;
        }

	}
}
