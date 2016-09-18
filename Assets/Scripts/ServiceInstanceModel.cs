using UnityEngine;
using System.Collections;
using thelab.mvc;

public class ServiceInstanceModel : Model<DingoApplication> {
	public int port;
	public bool highlight;
	bool highlightBeforeNotification;

	// Booleans emulating action buttons - if set, trigger action and reset bool
	public bool sendBaseBackup;
	public bool sendReplicaBackup;

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
		if (sendReplicaBackup) {
			sendReplicaBackup = false;
			app.Notify ("data-flow.replica-backup.request", this);
		}
	}

	public bool Equals(ServiceInstanceModel other) {
		return port == other.port;
	}

	public override string ToString() {
		return "ServiceInstanceModel(" + name + "," + port + ")";
	}
}
