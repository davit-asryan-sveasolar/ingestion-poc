namespace Vendors.Huawei.Client.Models;

public class Plant
{
    public Plant(float capacity, string contactMethod, string contactPerson, string gridConnectionDate, string latitude, string longitude, string plantAddress, string plantCode, string plantName)
    {
        Capacity = capacity;
        ContactMethod = contactMethod;
        ContactPerson = contactPerson;
        GridConnectionDate = gridConnectionDate;
        Latitude = latitude;
        Longitude = longitude;
        PlantAddress = plantAddress;
        PlantCode = plantCode;
        PlantName = plantName;
    }

    public float Capacity { get; }
    public string ContactMethod { get; }
    public string ContactPerson { get; }
    public string GridConnectionDate { get; }
    public string Latitude { get; }
    public string Longitude { get; }
    public string PlantAddress { get; }
    public string PlantCode { get; }
    public string PlantName { get; }
}