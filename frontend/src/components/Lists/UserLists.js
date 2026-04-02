import { useState, useEffect } from 'react';
import { getLists, createList, deleteList } from '../../api/listApi';
import { getCars } from '../../api/carApi';
import { Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import { FaPlus, FaTrash, FaCar } from 'react-icons/fa';
import './Lists.css';

const UserLists = () => {
  const [lists, setLists] = useState([]);
  const [loading, setLoading] = useState(true);
  const [newListName, setNewListName] = useState('');
  const [expandedList, setExpandedList] = useState(null);
  const [listCars, setListCars] = useState({});

  const fetchLists = async () => {
    try {
      const res = await getLists();
      setLists(res.data || []);
    } catch {
      toast.error('Listeler yuklenemedi');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchLists();
  }, []);

  const handleCreate = async (e) => {
    e.preventDefault();
    if (!newListName.trim()) return;
    try {
      await createList({ listName: newListName });
      setNewListName('');
      fetchLists();
      toast.success('Liste olusturuldu');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Olusturulamadi');
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Listeyi silmek istiyor musunuz?')) return;
    try {
      await deleteList(id);
      fetchLists();
      toast.success('Liste silindi');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Silinemedi');
    }
  };

  const toggleExpand = async (list) => {
    if (expandedList === list.id) {
      setExpandedList(null);
      return;
    }
    setExpandedList(list.id);
    if (!listCars[list.id] && list.items && list.items.length > 0) {
      try {
        const res = await getCars({ limit: 100, offset: 0 });
        const allCars = res.data.data || [];
        const matched = allCars.filter((c) => list.items.includes(c.id));
        setListCars((prev) => ({ ...prev, [list.id]: matched }));
      } catch {
        // silently fail
      }
    }
  };

  if (loading) return <div className="loading"><div className="spinner"></div></div>;

  return (
    <div className="lists-page">
      <div className="page-header">
        <h1>Listelerim</h1>
      </div>

      <form className="create-list-form" onSubmit={handleCreate}>
        <input
          value={newListName}
          onChange={(e) => setNewListName(e.target.value)}
          placeholder="Yeni liste adi..."
          required
        />
        <button type="submit" className="btn btn-primary" style={{ width: 'auto', padding: '10px 20px' }}>
          <FaPlus /> Olustur
        </button>
      </form>

      {lists.length === 0 ? (
        <div className="empty-state"><p>Henuz listeniz yok</p></div>
      ) : (
        <div className="lists-container">
          {lists.map((list) => (
            <div key={list.id} className="list-card">
              <div className="list-card-header" onClick={() => toggleExpand(list)}>
                <div>
                  <h3>{list.listName}</h3>
                  <span className="list-count">{list.items?.length || 0} arac</span>
                </div>
                <button
                  className="btn-icon btn-icon-danger"
                  onClick={(e) => { e.stopPropagation(); handleDelete(list.id); }}
                  title="Sil"
                >
                  <FaTrash />
                </button>
              </div>
              {expandedList === list.id && (
                <div className="list-cars">
                  {(!list.items || list.items.length === 0) ? (
                    <p className="no-items">Bu listede henuz arac yok</p>
                  ) : listCars[list.id] ? (
                    listCars[list.id].map((car) => (
                      <Link key={car.id} to={`/cars/${car.id}`} className="list-car-item">
                        <FaCar />
                        <span>{car.marka} {car.seri} {car.model}</span>
                        <span className="list-car-price">
                          {new Intl.NumberFormat('tr-TR').format(car.fiyat)} TL
                        </span>
                      </Link>
                    ))
                  ) : (
                    <div className="loading"><div className="spinner"></div></div>
                  )}
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default UserLists;
