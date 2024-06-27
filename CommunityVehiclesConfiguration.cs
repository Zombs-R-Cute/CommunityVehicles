using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;

namespace Zombs_R_Cute_Community_Vehicles
{
    public class CommunityVehiclesConfiguration: IRocketPluginConfiguration
    {
        [XmlArray(ElementName = "VehicleIds"), XmlArrayItem(ElementName = "Id")]
        public HashSet<ushort> VehicleIds;
        
        public void LoadDefaults()
        {
            VehicleIds = new HashSet<ushort>() { 186, 187 };
        }
    }
}