import { useState } from 'react';
import { FaSearch, FaTimes } from 'react-icons/fa';
import './Cars.css';
import CAR_BRANDS from '../../constants/carBrands';
const FUEL_TYPES = ['Benzin', 'Dizel', 'LPG', 'Elektrik', 'Hibrit'];
const TRANSMISSION_TYPES = ['Manuel', 'Otomatik', 'Yari Otomatik'];
const BODY_TYPES = ['Sedan', 'Hatchback', 'SUV', 'Coupe', 'Cabrio', 'Station Wagon', 'MPV', 'Pick-up'];

const CarFilter = ({ onFilter }) => {
  const [filters, setFilters] = useState({
    brand: '',
    seri: '',
    model: '',
    location: '',
    minPrice: '',
    maxPrice: '',
  });
  const [showFilters, setShowFilters] = useState(false);

  const handleChange = (e) => {
    setFilters({ ...filters, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const params = {};
    Object.entries(filters).forEach(([key, value]) => {
      if (value) params[key] = value;
    });
    onFilter(params);
  };

  const handleClear = () => {
    setFilters({ brand: '', seri: '', model: '', location: '', minPrice: '', maxPrice: '' });
    onFilter({});
  };

  return (
    <div className="car-filter">
      <button className="filter-toggle" onClick={() => setShowFilters(!showFilters)}>
        <FaSearch /> {showFilters ? 'Filtreleri Gizle' : 'Filtrele'}
      </button>

      {showFilters && (
        <form onSubmit={handleSubmit} className="filter-form">
          <div className="filter-grid">
            <div className="form-group">
              <label>Marka</label>
              <select name="brand" value={filters.brand} onChange={handleChange}>
                <option value="">Tumu</option>
                {CAR_BRANDS.map((b) => (
                  <option key={b} value={b}>{b}</option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label>Seri</label>
              <input name="seri" value={filters.seri} onChange={handleChange} placeholder="orn: A3, Corolla" />
            </div>
            <div className="form-group">
              <label>Model</label>
              <input name="model" value={filters.model} onChange={handleChange} placeholder="orn: 1.6 TDI" />
            </div>
            <div className="form-group">
              <label>Sehir</label>
              <input name="location" value={filters.location} onChange={handleChange} placeholder="orn: Istanbul" />
            </div>
            <div className="form-group">
              <label>Min Fiyat</label>
              <input type="number" name="minPrice" value={filters.minPrice} onChange={handleChange} placeholder="0" />
            </div>
            <div className="form-group">
              <label>Max Fiyat</label>
              <input type="number" name="maxPrice" value={filters.maxPrice} onChange={handleChange} placeholder="10.000.000" />
            </div>
          </div>
          <div className="filter-actions">
            <button type="submit" className="btn btn-primary">
              <FaSearch /> Ara
            </button>
            <button type="button" className="btn btn-secondary" onClick={handleClear}>
              <FaTimes /> Temizle
            </button>
          </div>
        </form>
      )}
    </div>
  );
};

export default CarFilter;
