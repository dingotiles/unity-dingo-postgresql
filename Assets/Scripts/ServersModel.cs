using UnityEngine;
using System.Collections;
using thelab.mvc;

public class ServersModel : Model<DingoApplication> {
	public enum AvailabilityZone { AvailabilityZone1, AvailabilityZone2 };
	[System.Serializable]
	public class Server {
		public AvailabilityZone az;
		public string serverLabel;
		public bool available = true;
	}

	public Server[] servers;

	public Server RandomAvailableServer(ServersModel.AvailabilityZone az) {
		int azServersCount = 0;
		for (int i = 0; i < servers.Length; i++) {
			if (servers [i].az == az && servers[i].available) {
				azServersCount++;
			}
		}

		int azServerNum = Random.Range (0, azServersCount);
		azServersCount = 0;
		for (int i = 0; i < servers.Length; i++) {
			if (servers [i].az == az && servers[i].available) {
				if (azServersCount == azServerNum) {
					return servers [i];
				}
				azServersCount++;
			}
		}
		return null;
	}
}
