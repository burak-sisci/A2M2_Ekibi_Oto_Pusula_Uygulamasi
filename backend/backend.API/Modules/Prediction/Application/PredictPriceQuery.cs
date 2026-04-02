using backend.API.Modules.Cars.Domain;
using backend.API.Modules.Prediction.Domain;
using MediatR;

namespace backend.API.Modules.Prediction.Application;

public record PredictPriceQuery(Car Car) : IRequest<PricePredictionResponse>;