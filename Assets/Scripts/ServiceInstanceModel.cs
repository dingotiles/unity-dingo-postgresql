using UnityEngine;
using System.Collections;
using thelab.mvc;

public class ServiceInstanceModel : Model<DingoApplication> {
	public int port;

	public int dataPacketsRecv = 0;
	int dataPacketsPerWAL { get { return app.model.ServiceInstances.dataPacketsPerWAL; } }
	public int walSegments { get { return dataPacketsRecv / dataPacketsPerWAL; } }

	public bool highlight;
	bool highlightBeforeNotification;

	// Booleans emulating action buttons - if set, trigger action and reset bool
	public bool sendBaseBackup;
	public bool sendData;
	public bool recreateFromBackup;

	public ServersModel.Server leaderServer;
	public ServersModel.Server replicaServer;


	void Update() {
		if (highlight != highlightBeforeNotification) {
			highlightBeforeNotification = highlight;
			app.Notify ("service-instance.change.highlight", this);
		}
		if (sendBaseBackup) {
			sendBaseBackup = false;
			app.Notify ("data-flow.base-backup.request", this);
		}
		if (sendData) {
			sendData = false;
			ReceivedDataPacket ();
			app.Notify ("data-flow.data.request", this);
		}
		if (recreateFromBackup) {
			recreateFromBackup = false;
			app.Notify ("service-instance.recreate.request", this);
		}
	}

	public void AssignServers()
	{
		leaderServer.az = ServersModel.AvailabilityZone.AvailabilityZone1;
		leaderServer.serverLabel = RandomAvailableServer (leaderServer.az);
		replicaServer.az = ServersModel.AvailabilityZone.AvailabilityZone2;
		replicaServer.serverLabel = RandomAvailableServer (replicaServer.az);
	}

	public void ReceivedDataPacket()
	{
		dataPacketsRecv++;
		if (dataPacketsRecv % dataPacketsPerWAL == 0) {
			app.Notify ("data-flow.wal.request", this);
		}
	}

	public bool Equals(ServiceInstanceModel other) {
		return port == other.port;
	}

	public override string ToString() {
		return "ServiceInstanceModel(" + name + "," + port + ")";
	}

	string RandomAvailableServer(ServersModel.AvailabilityZone az) {
		ServersModel.Server server = app.model.Servers.RandomAvailableServer (az);
		return server.serverLabel;
	}
}
