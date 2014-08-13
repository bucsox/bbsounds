using Microsoft.Phone.Net.NetworkInformation;
using System;
using Tonality.Services.Interfaces;
using Windows.Networking.Connectivity;

namespace Tonality.Services
{
    public class NetworkService : INetworkService
    {
        public bool IsConnectionAvailable
        {
            get
            {
                if (Environment.OSVersion.Version.Minor >= 10)
                {
                    ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();

                    if (profile == null)
                    {
                        return false;
                    }

                    return profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
                }
                else
                {
                    return NetworkInterface.GetIsNetworkAvailable();
                }
            }
        }
    }
}
