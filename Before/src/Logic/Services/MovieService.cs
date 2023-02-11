using System;
using Logic.Entities;
using Logic.ValueObjects;

namespace Logic.Services;

public class MovieService
{
    public ExpirationDate GetExpirationDate(LicensingModel licensingModel)
    {
        ExpirationDate result;

        switch (licensingModel)
        {
            case LicensingModel.TwoDays:
                result = (ExpirationDate)DateTime.UtcNow.AddDays(2);
                break;

            case LicensingModel.LifeLong:
                result = ExpirationDate.Infinity;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        return result;
    }
}
