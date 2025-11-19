using System;

public static class AppEvents
{
    public static event Action RentalsUpdated;
    public static event Action PaymentsUpdated;
    public static event Action VehiclesUpdated;
    public static event Action CustomersUpdated;  

    public static void RaiseRentalsUpdated()
    {
        RentalsUpdated?.Invoke();
    }

    public static void RaisePaymentsUpdated()
    {
        PaymentsUpdated?.Invoke();
    }

    public static void RaiseVehiclesUpdated()
    {
        VehiclesUpdated?.Invoke();
    }

    public static void RaiseCustomersUpdated() 
    {
        CustomersUpdated?.Invoke();
    }
}
