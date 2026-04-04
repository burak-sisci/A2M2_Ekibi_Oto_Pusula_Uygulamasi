import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getCars, deleteCar } from '../../api/carApi';
import { addItemToList, getLists } from '../../api/listApi';
import { useAuth } from '../../context/AuthContext';
import CommentSection from '../Comments/CommentSection';
import { toast } from 'react-toastify';
import { FaHeart, FaEdit, FaTrash, FaMapMarkerAlt, FaGasPump, FaCog, FaRoad, FaTachometerAlt, FaCarSide } from 'react-icons/fa';
import './Cars.css';

// Backend BsonElement adlariyla eslestirilmis panel labels
const PANEL_LABELS = {
  frontBumper: '\u00d6n Tampon',
  engineHood: 'Motor Kaputu',
  roof: 'Tavan',
  rearBumper: 'Arka Tampon',
  rearHood: 'Arka Kaput',
  leftFrontFender: 'Sol \u00d6n \u00c7amurluk',
  leftFrontDoor: 'Sol \u00d6n Kap\u0131',
  leftRearDoor: 'Sol Arka Kap\u0131',
  leftRearFender: 'Sol Arka \u00c7amurluk',
  rightFrontFender: 'Sa\u011f \u00d6n \u00c7amurluk',
  rightFrontDoor: 'Sa\u011f \u00d6n Kap\u0131',
  rightRearDoor: 'Sa\u011f Arka Kap\u0131',
  rightRearFender: 'Sa\u011f Arka \u00c7amurluk',
};

const panelColor = (val) => {
  if (!val || val === 'Orijinal') return 'panel-original';
  if (val === 'Boyali') return 'panel-painted';
  return 'panel-changed';
};

const CarDetail = () => {
  const { id } = useParams();
  const { user } = useAuth();
  const navigate = useNavigate();
  const [car, setCar] = useState(null);
  const [loading, setLoading] = useState(true);
  const [activeImg, setActiveImg] = useState(0);
  const [lists, setLists] = useState([]);
  const [showListModal, setShowListModal] = useState(false);

  useEffect(() => {
    const fetchCar = async () => {
      try {
        const res = await getCars({ limit: 100, offset: 0 });
        const found = (res.data.data || []).find((c) => c.id === id);
        if (found) setCar(found);
        else toast.error('Ilan bulunamadi');
      } catch {
        toast.error('Ilan yuklenemedi');
      } finally {
        setLoading(false);
      }
    };
    fetchCar();
  }, [id]);

  const handleDelete = async () => {
    if (!window.confirm('Bu ilani silmek istediginize emin misiniz?')) return;
    try {
      await deleteCar(id);
      toast.success('Ilan silindi');
      navigate('/');
    } catch (err) {
      toast.error(err.response?.data?.message || 'Silme basarisiz');
    }
  };

  const handleFavorite = async () => {
    if (!user) {
      toast.info('Favorilere eklemek icin giris yapin');
      return;
    }
    try {
      const res = await getLists();
      setLists(res.data || []);
      setShowListModal(true);
    } catch {
      toast.error('Listeler yuklenemedi');
    }
  };

  const addToList = async (listId) => {
    try {
      await addItemToList(listId, { carId: id });
      toast.success('Listeye eklendi');
      setShowListModal(false);
    } catch (err) {
      toast.error(err.response?.data?.message || 'Eklenemedi');
    }
  };

  if (loading) return <div className="loading"><div className="spinner"></div></div>;
  if (!car) return <div className="empty-state"><p>Ilan bulunamadi</p></div>;

  const formatPrice = (p) => new Intl.NumberFormat('tr-TR').format(p) + ' TL';
  const formatKm = (k) => new Intl.NumberFormat('tr-TR').format(k) + ' km';
  const images = car.resimler && car.resimler.length > 0 ? car.resimler : [];
  const isOwner = user && car.ilanSahibi === user.id;
  const panels = car.boyaliDegisen || car.boyaliveDegisen;

  return (
    <div className="car-detail-page">
      <div className="car-detail-container">
        {/* Image Gallery */}
        <div className="car-gallery">
          <div className="gallery-main">
            {images.length > 0 ? (
              <img src={images[activeImg]} alt={car.marka} />
            ) : (
              <div className="no-image">Resim Yok</div>
            )}
          </div>
          {images.length > 1 && (
            <div className="gallery-thumbs">
              {images.map((img, i) => (
                <img
                  key={i}
                  src={img}
                  alt={`${i + 1}`}
                  className={i === activeImg ? 'active' : ''}
                  onClick={() => setActiveImg(i)}
                />
              ))}
            </div>
          )}
        </div>

        {/* Car Info */}
        <div className="car-info">
          <div className="car-info-header">
            <div>
              <h1>{car.marka} {car.seri} {car.model}</h1>
              <p className="car-detail-price">{formatPrice(car.fiyat)}</p>
            </div>
            <div className="car-actions">
              <button className="btn-icon" onClick={handleFavorite} title="Favorilere Ekle">
                <FaHeart />
              </button>
              {isOwner && (
                <>
                  <button className="btn-icon" onClick={() => navigate(`/cars/${id}/edit`)} title="Duzenle">
                    <FaEdit />
                  </button>
                  <button className="btn-icon btn-icon-danger" onClick={handleDelete} title="Sil">
                    <FaTrash />
                  </button>
                </>
              )}
            </div>
          </div>

          <div className="specs-grid">
            <div className="spec-item">
              <FaRoad className="spec-icon" />
              <div>
                <span className="spec-label">Kilometre</span>
                <span className="spec-value">{formatKm(car.kilometre)}</span>
              </div>
            </div>
            <div className="spec-item">
              <FaGasPump className="spec-icon" />
              <div>
                <span className="spec-label">Yakit</span>
                <span className="spec-value">{car.yakitTipi}</span>
              </div>
            </div>
            <div className="spec-item">
              <FaCog className="spec-icon" />
              <div>
                <span className="spec-label">Vites</span>
                <span className="spec-value">{car.vitesTipi}</span>
              </div>
            </div>
            <div className="spec-item">
              <FaTachometerAlt className="spec-icon" />
              <div>
                <span className="spec-label">Motor Gucu</span>
                <span className="spec-value">{car.motorGucu} HP</span>
              </div>
            </div>
            <div className="spec-item">
              <FaCarSide className="spec-icon" />
              <div>
                <span className="spec-label">Kasa Tipi</span>
                <span className="spec-value">{car.kasaTipi}</span>
              </div>
            </div>
            <div className="spec-item">
              <FaMapMarkerAlt className="spec-icon" />
              <div>
                <span className="spec-label">Konum</span>
                <span className="spec-value">{car.konum}</span>
              </div>
            </div>
          </div>

          <div className="detail-section">
            <h3>Detaylar</h3>
            <div className="detail-table">
              <div className="detail-row"><span>Yil</span><span>{car.yil}</span></div>
              <div className="detail-row"><span>Renk</span><span>{car.renk}</span></div>
              <div className="detail-row"><span>Motor Hacmi</span><span>{car.motorHacmi} cc</span></div>
              <div className="detail-row"><span>Cekis</span><span>{car.cekis}</span></div>
              <div className="detail-row"><span>Arac Durumu</span><span>{car.aracDurumu}</span></div>
              <div className="detail-row"><span>Kimden</span><span>{car.kimden}</span></div>
              <div className="detail-row"><span>Ort. Yakit Tuketimi</span><span>{car.ortalamaYakitTuketim} L/100km</span></div>
              <div className="detail-row"><span>Yakit Deposu</span><span>{car.yakitDeposu} L</span></div>
              <div className="detail-row"><span>Agir Hasar Kaydi</span><span>{car.agirHasarKaydi ? 'Var' : 'Yok'}</span></div>
              <div className="detail-row"><span>Takasa Uygun</span><span>{car.takasaUygun ? 'Evet' : 'Hayir'}</span></div>
            </div>
          </div>

          {/* Panel Status */}
          {panels && (
            <div className="detail-section">
              <h3>Boya ve Degisen</h3>
              <div className="panel-grid">
                {Object.entries(PANEL_LABELS).map(([key, label]) => (
                  <div key={key} className={`panel-item ${panelColor(panels[key])}`}>
                    <span className="panel-name">{label}</span>
                    <span className="panel-status">{panels[key] || 'Orijinal'}</span>
                  </div>
                ))}
              </div>
              <div className="panel-legend">
                <span className="legend-item"><span className="dot dot-green"></span> Orijinal</span>
                <span className="legend-item"><span className="dot dot-yellow"></span> Boyali</span>
                <span className="legend-item"><span className="dot dot-red"></span> Degismis</span>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* Comments */}
      <CommentSection carId={id} />

      {/* List Modal */}
      {showListModal && (
        <div className="modal-overlay" onClick={() => setShowListModal(false)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <h3>Listeye Ekle</h3>
            {lists.length === 0 ? (
              <p>Henuz listeniz yok</p>
            ) : (
              <div className="list-options">
                {lists.map((list) => (
                  <button key={list.id} className="list-option-btn" onClick={() => addToList(list.id)}>
                    {list.listName}
                  </button>
                ))}
              </div>
            )}
            <button className="btn btn-secondary" onClick={() => setShowListModal(false)} style={{ marginTop: 12 }}>
              Kapat
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default CarDetail;
