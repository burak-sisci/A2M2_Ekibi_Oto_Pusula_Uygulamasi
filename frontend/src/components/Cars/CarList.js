import { useState, useEffect } from 'react';
import { getCars } from '../../api/carApi';
import CarCard from './CarCard';
import CarFilter from './CarFilter';
import { toast } from 'react-toastify';
import './Cars.css';

const CarList = () => {
  const [cars, setCars] = useState([]);
  const [loading, setLoading] = useState(true);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(0);
  const [filters, setFilters] = useState({});
  const limit = 12;

  const fetchCars = async (filterParams = filters, offset = page * limit) => {
    setLoading(true);
    try {
      const res = await getCars({ limit, offset, ...filterParams });
      setCars(res.data.data || []);
      setTotalCount(res.data.totalCount || 0);
    } catch (err) {
      toast.error('Ilanlar yuklenemedi');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCars(filters, page * limit);
  }, [page]);

  const handleFilter = (params) => {
    setFilters(params);
    setPage(0);
    fetchCars(params, 0);
  };

  const totalPages = Math.ceil(totalCount / limit);

  return (
    <div className="car-list-page">
      <div className="page-header">
        <h1>Arac Ilanlari</h1>
        <p>{totalCount} ilan bulundu</p>
      </div>

      <CarFilter onFilter={handleFilter} />

      {loading ? (
        <div className="loading">
          <div className="spinner"></div>
          <p>Ilanlar yukleniyor...</p>
        </div>
      ) : cars.length === 0 ? (
        <div className="empty-state">
          <p>Ilan bulunamadi</p>
        </div>
      ) : (
        <>
          <div className="car-grid">
            {cars.map((car) => (
              <CarCard key={car.id} car={car} />
            ))}
          </div>

          {totalPages > 1 && (
            <div className="pagination">
              <button
                disabled={page === 0}
                onClick={() => setPage(page - 1)}
                className="btn btn-secondary"
              >
                Onceki
              </button>
              <span className="page-info">
                {page + 1} / {totalPages}
              </span>
              <button
                disabled={page >= totalPages - 1}
                onClick={() => setPage(page + 1)}
                className="btn btn-secondary"
              >
                Sonraki
              </button>
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default CarList;
