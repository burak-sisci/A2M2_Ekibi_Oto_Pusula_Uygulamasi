import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { createCar, updateCar, getCars, uploadImage } from '../../api/carApi';
import { FaUpload, FaLink } from 'react-icons/fa';
import { toast } from 'react-toastify';
import CAR_BRANDS from '../../constants/carBrands';
import './Cars.css';

// Backend enum degerlerine birebir uyumlu
const INITIAL_PANELS = {
  '\u00d6nTampon': 'Orijinal',
  'MotorKaputu': 'Orijinal',
  'Tavan': 'Orijinal',
  'ArkaTampon': 'Orijinal',
  'ArkaKaput': 'Orijinal',
  'Sol\u00d6n\u00c7amurluk': 'Orijinal',
  'Sol\u00d6nKapi': 'Orijinal',
  'SolArkaKapi': 'Orijinal',
  'SolArka\u00c7amurluk': 'Orijinal',
  'Sa\u011f\u00d6n\u00c7amurluk': 'Orijinal',
  'Sa\u011f\u00d6nKapi': 'Orijinal',
  'Sa\u011fArkaKapi': 'Orijinal',
  'Sa\u011fArka\u00c7amurluk': 'Orijinal',
};

const PANEL_LABELS = {
  '\u00d6nTampon': '\u00d6n Tampon',
  'MotorKaputu': 'Motor Kaputu',
  'Tavan': 'Tavan',
  'ArkaTampon': 'Arka Tampon',
  'ArkaKaput': 'Arka Kaput',
  'Sol\u00d6n\u00c7amurluk': 'Sol \u00d6n \u00c7amurluk',
  'Sol\u00d6nKapi': 'Sol \u00d6n Kap\u0131',
  'SolArkaKapi': 'Sol Arka Kap\u0131',
  'SolArka\u00c7amurluk': 'Sol Arka \u00c7amurluk',
  'Sa\u011f\u00d6n\u00c7amurluk': 'Sa\u011f \u00d6n \u00c7amurluk',
  'Sa\u011f\u00d6nKapi': 'Sa\u011f \u00d6n Kap\u0131',
  'Sa\u011fArkaKapi': 'Sa\u011f Arka Kap\u0131',
  'Sa\u011fArka\u00c7amurluk': 'Sa\u011f Arka \u00c7amurluk',
};

const PANEL_OPTIONS = [
  { value: 'Orijinal', label: 'Orijinal' },
  { value: 'Boyali', label: 'Boyal\u0131' },
  { value: 'De\u011fi\u015fmi\u015f', label: 'De\u011fi\u015fmi\u015f' },
];

// Backend enum: Benzin, Dizel, Elektrik, Hibrit, LPG, Benzin_LPG
const FUEL_TYPES = [
  { value: 'Benzin', label: 'Benzin' },
  { value: 'Dizel', label: 'Dizel' },
  { value: 'LPG', label: 'LPG' },
  { value: 'Elektrik', label: 'Elektrik' },
  { value: 'Hibrit', label: 'Hibrit' },
  { value: 'Benzin_LPG', label: 'Benzin & LPG' },
];

// Backend enum: Düz, Otomatik, YariOtomatik
const TRANSMISSION_TYPES = [
  { value: 'D\u00fcz', label: 'D\u00fcz (Manuel)' },
  { value: 'Otomatik', label: 'Otomatik' },
  { value: 'YariOtomatik', label: 'Yar\u0131 Otomatik' },
];

// Backend enum: Sedan, Hatchback, SUV, Crossover, Coupe, Cabrio, StationWagon, Minivan, Pickup, Van
const BODY_TYPES = [
  { value: 'Sedan', label: 'Sedan' },
  { value: 'Hatchback', label: 'Hatchback' },
  { value: 'SUV', label: 'SUV' },
  { value: 'Crossover', label: 'Crossover' },
  { value: 'Coupe', label: 'Coupe' },
  { value: 'Cabrio', label: 'Cabrio' },
  { value: 'StationWagon', label: 'Station Wagon' },
  { value: 'Minivan', label: 'Minivan' },
  { value: 'Pickup', label: 'Pickup' },
  { value: 'Van', label: 'Van (Panelvan)' },
];

// Backend enum: ÖndenÇekiş, Arkadanİtiş, DörtÇeker, DörtcarpiDört
const DRIVE_TYPES = [
  { value: '\u00d6nden\u00c7eki\u015f', label: '\u00d6nden \u00c7eki\u015f' },
  { value: 'Arkadan\u0130ti\u015f', label: 'Arkadan \u0130ti\u015f' },
  { value: 'D\u00f6rt\u00c7eker', label: 'D\u00f6rt \u00c7eker' },
  { value: 'D\u00f6rtcarpiD\u00f6rt', label: '4x4' },
];

// Backend enum: Sifir, İkinciEl
const VEHICLE_CONDITIONS = [
  { value: 'Sifir', label: 'S\u0131f\u0131r' },
  { value: '\u0130kinciEl', label: '\u0130kinci El' },
];

// Backend enum: Sahibinden, Galeriden
const SELLER_TYPES = [
  { value: 'Sahibinden', label: 'Sahibinden' },
  { value: 'Galeriden', label: 'Galeriden' },
];

const CarForm = () => {
  const { id } = useParams();
  const isEdit = Boolean(id);
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [imageUrl, setImageUrl] = useState('');

  const [form, setForm] = useState({
    marka: '', seri: '', model: '', yil: new Date().getFullYear(), fiyat: '',
    kilometre: '', vitesTipi: 'D\u00fcz', yakitTipi: 'Benzin', kasaTipi: 'Sedan',
    renk: '', motorHacmi: '', motorGucu: '', cekis: '\u00d6nden\u00c7eki\u015f',
    aracDurumu: '\u0130kinciEl', ortalamaYakitTuketim: '', yakitDeposu: '',
    agirHasarKaydi: false, takasaUygun: false,
    kimden: 'Sahibinden', konum: '', resimler: [],
    boyaliDegisen: { ...INITIAL_PANELS },
  });

  useEffect(() => {
    if (isEdit) {
      const fetchCar = async () => {
        try {
          const res = await getCars({ limit: 100, offset: 0 });
          const found = (res.data.data || []).find((c) => c.id === id);
          if (found) {
            setForm({
              marka: found.marka || '', seri: found.seri || '', model: found.model || '',
              yil: found.yil || new Date().getFullYear(), fiyat: found.fiyat || '',
              kilometre: found.kilometre || '', vitesTipi: found.vitesTipi || 'D\u00fcz',
              yakitTipi: found.yakitTipi || 'Benzin', kasaTipi: found.kasaTipi || 'Sedan',
              renk: found.renk || '', motorHacmi: found.motorHacmi || '', motorGucu: found.motorGucu || '',
              cekis: found.cekis || '\u00d6nden\u00c7eki\u015f',
              aracDurumu: found.aracDurumu || '\u0130kinciEl',
              ortalamaYakitTuketim: found.ortalamaYakitTuketim || '',
              yakitDeposu: found.yakitDeposu || '',
              agirHasarKaydi: found.agirHasarKaydi || false,
              takasaUygun: found.takasaUygun || false,
              kimden: found.kimden || 'Sahibinden',
              konum: found.konum || '', resimler: found.resimler || [],
              boyaliDegisen: found.boyaliDegisen || { ...INITIAL_PANELS },
            });
          }
        } catch {
          toast.error('Ilan yuklenemedi');
        }
      };
      fetchCar();
    }
  }, [id, isEdit]);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === 'checkbox' ? checked : value });
  };

  const handlePanelChange = (panel, value) => {
    setForm({
      ...form,
      boyaliDegisen: { ...form.boyaliDegisen, [panel]: value },
    });
  };

  const addImageUrl = () => {
    if (imageUrl.trim()) {
      setForm({ ...form, resimler: [...form.resimler, imageUrl.trim()] });
      setImageUrl('');
    }
  };

  const handleFileUpload = async (e) => {
    const files = Array.from(e.target.files);
    for (const file of files) {
      try {
        const res = await uploadImage(file);
        setForm((prev) => ({ ...prev, resimler: [...prev.resimler, res.data.url] }));
        toast.success(`${file.name} yuklendi`);
      } catch (err) {
        toast.error(err.response?.data?.message || `${file.name} yuklenemedi`);
      }
    }
    e.target.value = '';
  };

  const removeImage = (index) => {
    setForm({ ...form, resimler: form.resimler.filter((_, i) => i !== index) });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    const payload = {
      marka: form.marka,
      seri: form.seri,
      model: form.model,
      yil: Number(form.yil),
      fiyat: Number(form.fiyat),
      kilometre: Number(form.kilometre),
      vitesTipi: form.vitesTipi,
      yakitTipi: form.yakitTipi,
      kasaTipi: form.kasaTipi,
      renk: form.renk,
      motorHacmi: Number(form.motorHacmi) || 0,
      motorGucu: Number(form.motorGucu) || 0,
      cekis: form.cekis,
      aracDurumu: form.aracDurumu,
      ortalamaYakitTuketim: Number(form.ortalamaYakitTuketim) || 0,
      yakitDeposu: Number(form.yakitDeposu) || 0,
      agirHasarKaydi: form.agirHasarKaydi,
      takasaUygun: form.takasaUygun,
      kimden: form.kimden,
      resimler: form.resimler,
      konum: form.konum,
      boyaliDegisen: form.boyaliDegisen,
    };

    try {
      if (isEdit) {
        await updateCar(id, payload);
        toast.success('Ilan guncellendi');
      } else {
        await createCar(payload);
        toast.success('Ilan olusturuldu');
      }
      navigate('/');
    } catch (err) {
      toast.error(err.response?.data?.message || err.response?.data?.title || 'Islem basarisiz');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="car-form-page">
      <form className="car-form" onSubmit={handleSubmit}>
        <h2>{isEdit ? 'Ilani Duzenle' : 'Yeni Ilan Olustur'}</h2>

        {/* Basic Info */}
        <div className="form-section">
          <h3>Temel Bilgiler</h3>
          <div className="form-grid">
            <div className="form-group">
              <label>Marka *</label>
              <select name="marka" value={form.marka} onChange={handleChange} required>
                <option value="">Seciniz</option>
                {CAR_BRANDS.map((b) => <option key={b} value={b}>{b}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label>Seri *</label>
              <input name="seri" value={form.seri} onChange={handleChange} required placeholder="orn: A3, Corolla" />
            </div>
            <div className="form-group">
              <label>Model *</label>
              <input name="model" value={form.model} onChange={handleChange} required placeholder="orn: 1.6 TDI" />
            </div>
            <div className="form-group">
              <label>Yil *</label>
              <input type="number" name="yil" value={form.yil} onChange={handleChange} required min={1950} max={2026} />
            </div>
            <div className="form-group">
              <label>Fiyat (TL) *</label>
              <input type="number" name="fiyat" value={form.fiyat} onChange={handleChange} required min={0} />
            </div>
            <div className="form-group">
              <label>Kilometre *</label>
              <input type="number" name="kilometre" value={form.kilometre} onChange={handleChange} required min={0} />
            </div>
            <div className="form-group">
              <label>Renk</label>
              <input name="renk" value={form.renk} onChange={handleChange} placeholder="orn: Beyaz, Siyah" />
            </div>
          </div>
        </div>

        {/* Technical */}
        <div className="form-section">
          <h3>Teknik Ozellikler</h3>
          <div className="form-grid">
            <div className="form-group">
              <label>Yakit Tipi</label>
              <select name="yakitTipi" value={form.yakitTipi} onChange={handleChange}>
                {FUEL_TYPES.map((f) => <option key={f.value} value={f.value}>{f.label}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label>Vites Tipi</label>
              <select name="vitesTipi" value={form.vitesTipi} onChange={handleChange}>
                {TRANSMISSION_TYPES.map((t) => <option key={t.value} value={t.value}>{t.label}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label>Kasa Tipi</label>
              <select name="kasaTipi" value={form.kasaTipi} onChange={handleChange}>
                {BODY_TYPES.map((b) => <option key={b.value} value={b.value}>{b.label}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label>Motor Hacmi (cc)</label>
              <input type="number" name="motorHacmi" value={form.motorHacmi} onChange={handleChange} min={0} step="0.1" />
            </div>
            <div className="form-group">
              <label>Motor Gucu (HP)</label>
              <input type="number" name="motorGucu" value={form.motorGucu} onChange={handleChange} min={0} />
            </div>
            <div className="form-group">
              <label>Cekis</label>
              <select name="cekis" value={form.cekis} onChange={handleChange}>
                {DRIVE_TYPES.map((d) => <option key={d.value} value={d.value}>{d.label}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label>Ort. Yakit Tuketimi (L/100km)</label>
              <input type="number" name="ortalamaYakitTuketim" value={form.ortalamaYakitTuketim} onChange={handleChange} min={0} step="0.1" />
            </div>
            <div className="form-group">
              <label>Yakit Deposu (L)</label>
              <input type="number" name="yakitDeposu" value={form.yakitDeposu} onChange={handleChange} min={0} />
            </div>
          </div>
        </div>

        {/* Condition */}
        <div className="form-section">
          <h3>Durum</h3>
          <div className="form-grid">
            <div className="form-group">
              <label>Arac Durumu</label>
              <select name="aracDurumu" value={form.aracDurumu} onChange={handleChange}>
                {VEHICLE_CONDITIONS.map((v) => <option key={v.value} value={v.value}>{v.label}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label>Kimden</label>
              <select name="kimden" value={form.kimden} onChange={handleChange}>
                {SELLER_TYPES.map((s) => <option key={s.value} value={s.value}>{s.label}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label>Konum *</label>
              <input name="konum" value={form.konum} onChange={handleChange} required placeholder="orn: Istanbul" />
            </div>
            <div className="form-group checkbox-group">
              <label>
                <input type="checkbox" name="agirHasarKaydi" checked={form.agirHasarKaydi} onChange={handleChange} />
                Agir Hasar Kaydi Var
              </label>
            </div>
            <div className="form-group checkbox-group">
              <label>
                <input type="checkbox" name="takasaUygun" checked={form.takasaUygun} onChange={handleChange} />
                Takasa Uygun
              </label>
            </div>
          </div>
        </div>

        {/* Images */}
        <div className="form-section">
          <h3>Resimler ({form.resimler.length})</h3>

          {/* File Upload */}
          <div className="image-upload-area">
            <label className="file-upload-btn">
              <FaUpload /> Bilgisayardan Resim Sec
              <input
                type="file"
                accept="image/*"
                multiple
                onChange={handleFileUpload}
                style={{ display: 'none' }}
              />
            </label>
            <span className="upload-hint">JPG, PNG, WEBP - Maks 5MB</span>
          </div>

          {/* URL Input */}
          <div className="image-url-section">
            <p className="url-label"><FaLink /> veya URL ile ekle:</p>
            <div className="image-input-row">
              <input
                value={imageUrl}
                onChange={(e) => setImageUrl(e.target.value)}
                placeholder="https://ornek.com/resim.jpg"
              />
              <button type="button" className="btn btn-secondary" onClick={addImageUrl}>Ekle</button>
            </div>
          </div>

          {/* Preview */}
          {form.resimler.length > 0 && (
            <div className="image-preview-list">
              {form.resimler.map((img, i) => (
                <div key={i} className="image-preview-item">
                  <img src={img} alt={`${i + 1}`} />
                  <button type="button" onClick={() => removeImage(i)}>&times;</button>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Panel Status */}
        <div className="form-section">
          <h3>Boya ve Degisen</h3>
          <div className="panel-form-grid">
            {Object.entries(PANEL_LABELS).map(([key, label]) => (
              <div key={key} className="form-group">
                <label>{label}</label>
                <select
                  value={form.boyaliDegisen[key]}
                  onChange={(e) => handlePanelChange(key, e.target.value)}
                >
                  {PANEL_OPTIONS.map((o) => <option key={o.value} value={o.value}>{o.label}</option>)}
                </select>
              </div>
            ))}
          </div>
        </div>

        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? 'Kaydediliyor...' : isEdit ? 'Guncelle' : 'Ilan Ver'}
        </button>
      </form>
    </div>
  );
};

export default CarForm;
