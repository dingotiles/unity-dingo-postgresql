using UnityEngine;
using System.Collections;
using thelab.mvc;

public class ServiceInstancesModel : Model<DingoApplication> {
	public int maxHighlighted = 1;
	public int dataPacketsPerWAL = 4;
	public GameObject serviceInstancePrefab;

	public ServiceInstanceModel[] ServiceInstances { 
		get {
			if (p_service_instances != null) {
				return p_service_instances;
			}
			return p_service_instances = transform.GetComponentsInChildren<ServiceInstanceModel> ();
		}
	}
	ServiceInstanceModel[] p_service_instances;

	public bool addDemo;
	public bool addDemoAndHighlight;
	bool provisioningDemoServiceInstance;

	void Update() {
		if (addDemo && !provisioningDemoServiceInstance) {
			ProvisionDemoServiceInstance (false);
		}
		if (addDemoAndHighlight && !provisioningDemoServiceInstance) {
			ProvisionDemoServiceInstance (true);
		}
	}

	void ProvisionDemoServiceInstance(bool highlight) {
		provisioningDemoServiceInstance = true;

		GameObject serviceInstanceObj = Instantiate (serviceInstancePrefab, transform) as GameObject;
		ServiceInstanceModel serviceInstance = serviceInstanceObj.GetComponent<ServiceInstanceModel> ();
		serviceInstance.port = NextPort ();
		serviceInstance.name = "Demo " + serviceInstance.port;
		serviceInstance.highlight = highlight;

		p_service_instances = null;
		app.Notify ("service-instance.create.request", serviceInstance);
		app.Notify ("service-instances.update", this);

		addDemo = false;
		addDemoAndHighlight = false;
		provisioningDemoServiceInstance = false;
	}

	int lastAllocatedPort;
	int defaultFirstPort = 30000;
	int NextPort() {
		if (lastAllocatedPort <= 0) {
			// pre-populate latestAllocatedPort from existing ServiceInstances
			for (int i = 0; i < ServiceInstances.Length; i++) {
				if (ServiceInstances [i].port > lastAllocatedPort) {
					lastAllocatedPort = ServiceInstances [i].port;
				}
			}
		}
		if (lastAllocatedPort <= 0) {
			lastAllocatedPort = defaultFirstPort - 1;
		}
		lastAllocatedPort++;
		return lastAllocatedPort;
	}

}
