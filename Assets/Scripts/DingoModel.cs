using UnityEngine;
using System.Collections;
using thelab.mvc;

public class DingoModel : Model<DingoApplication> {
	public ServiceInstancesModel ServiceInstances { get { return p_service_instances = base.Assert<ServiceInstancesModel> (p_service_instances); } }
	ServiceInstancesModel p_service_instances;

	public ServersModel Servers { get { return p_servers = base.Assert<ServersModel> (p_servers); } }
	ServersModel p_servers;

}
