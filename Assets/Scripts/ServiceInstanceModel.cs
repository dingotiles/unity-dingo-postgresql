using UnityEngine;
using System.Collections;
using thelab.mvc;

public class ServiceInstanceModel : Model<DingoApplication> {
	public int port;
	public bool highlight;

	public bool sendBaseBackup;
	bool sendingDataFlow;
	public bool sendReplicaBackup;
	bool sendingReplicaBackup;

	public ServersModel.Server leaderServer;
	public ServersModel.Server replicaServer;

	bool highlightBeforeNotification;

	void Update() {
		if (highlight != highlightBeforeNotification) {
			highlightBeforeNotification = highlight;
			app.Notify ("service-instance.change.highlight", this);
		}
		if (sendBaseBackup && !sendingDataFlow) {
			SendBaseBackup ();
		}
		if (sendReplicaBackup && !sendingReplicaBackup) {
			SendReplicaBackup ();
		}
	}

	void SendBaseBackup() {
		sendingDataFlow = true;

		app.Notify ("data-flow.base-backup.request", this);

		sendBaseBackup = false;
		sendingDataFlow = false;
	}

	void SendReplicaBackup() {
		sendReplicaBackup = true;

		app.Notify ("data-flow.replica-backup.request", this);

		sendReplicaBackup = false;
		sendingReplicaBackup = false;
	}

	public bool Equals(ServiceInstanceModel other) {
		return port == other.port;
	}

	public override string ToString() {
		return "ServiceInstanceModel(" + name + "," + port + ")";
	}
}
