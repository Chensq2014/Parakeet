namespace Parakeet.Net.ROClient.Models
{
    public class ProjectGeofenceDeletedModel : ModelBase
    {
        public override string CommandName => "delete_project_geofence";

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }
    }
}