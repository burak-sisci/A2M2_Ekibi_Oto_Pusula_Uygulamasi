import { Link } from 'react-router-dom';
import { FaGasPump, FaCog, FaRoad, FaCalendarAlt } from 'react-icons/fa';
import './Cars.css';

const CarCard = ({ car }) => {
  const imageUrl = car.resimler && car.resimler.length > 0
    ? car.resimler[0]
    : 'https://via.placeholder.com/400x250/0f3460/e94560?text=Resim+Yok';

  const formatPrice = (price) => {
    return new Intl.NumberFormat('tr-TR').format(price) + ' TL';
  };

  const formatKm = (km) => {
    return new Intl.NumberFormat('tr-TR').format(km) + ' km';
  };

  return (
    <Link to={`/cars/${car.id}`} className="car-card">
      <div className="car-card-image">
        <img src={imageUrl} alt={`${car.marka} ${car.seri} ${car.model}`} />
        {car.agirHasarKaydi && <span className="badge badge-danger">Agir Hasar</span>}
        {car.takasaUygun && <span className="badge badge-info">Takasa Uygun</span>}
      </div>
      <div className="car-card-body">
        <h3 className="car-card-title">{car.marka} {car.seri} {car.model}</h3>
        <p className="car-card-price">{formatPrice(car.fiyat)}</p>
        <div className="car-card-specs">
          <span><FaCalendarAlt /> {car.yil}</span>
          <span><FaRoad /> {formatKm(car.kilometre)}</span>
          <span><FaGasPump /> {car.yakitTipi}</span>
          <span><FaCog /> {car.vitesTipi}</span>
        </div>
        <p className="car-card-location">{car.konum}</p>
      </div>
    </Link>
  );
};

export default CarCard;
