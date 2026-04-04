import { useState } from 'react';
import { predictPrice } from '../../api/predictionApi';
import { toast } from 'react-toastify';
import { FaChartLine } from 'react-icons/fa';
import './Prediction.css';
import CAR_BRANDS from '../../constants/carBrands';

const FUEL_TYPES = [
  { value: 'Benzin', label: 'Benzin' },
  { value: 'Dizel', label: 'Dizel' },
  { value: 'LPG', label: 'LPG' },
  { value: 'Elektrik', label: 'Elektrik' },
  { value: 'Hibrit', label: 'Hibrit' },
  { value: 'Benzin_LPG', label: 'Benzin & LPG' },
];

const TRANSMISSION_TYPES = [
  { value: 'D\u00fcz', label: 'D\u00fcz (Manuel)' },
  { value: 'Otomatik', label: 'Otomatik' },
  { value: 'YariOtomatik', label: 'Yar\u0131 Otomatik' },
];

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

const DRIVE_TYPES = [
  { value: '\u00d6nden\u00c7eki\u015f', label: '\u00d6nden \u00c7eki\u015f' },
  { value: 'Arkadan\u0130ti\u015f', label: 'Arkadan \u0130ti\u015f' },
  { value: 'D\u00f6rt\u00c7eker', label: 'D\u00f6rt \u00c7eker' },
  { value: 'D\u00f6rtcarpiD\u00f6rt', label: '4x4' },
];

const VEHICLE_CONDITIONS = [
  { value: 'Sifir', label: 'S\u0131f\u0131r' },
  { value: '\u0130kinciEl', label: '\u0130kinci El' },
];

const SELLER_TYPES = [
  { value: 'Sahibinden', label: 'Sahibinden' },
  { value: 'Galeriden', label: 'Galeriden' },
];

const PANEL_KEYS = [
  { key: '\u00d6nTampon', label: '\u00d6n Tampon' },
  { key: 'MotorKaputu', label: 'Motor Kaputu' },
  { key: 'Tavan', label: 'Tavan' },
  { key: 'ArkaTampon', label: 'Arka Tampon' },
  { key: 'ArkaKaput', label: 'Arka Kaput' },
  { key: 'Sol\u00d6n\u00c7amurluk', label: 'Sol \u00d6n \u00c7amurluk' },
  { key: 'Sol\u00d6nKapi', label: 'Sol \u00d6n Kap\u0131' },
  { key: 'SolArkaKapi', label: 'Sol Arka Kap\u0131' },
  { key: 'SolArka\u00c7amurluk', label: 'Sol Arka \u00c7amurluk' },
  { key: 'Sa\u011f\u00d6n\u00c7amurluk', label: 'Sa\u011f \u00d6n \u00c7amurluk' },
  { key: 'Sa\u011f\u00d6nKapi', label: 'Sa\u011f \u00d6n Kap\u0131' },
  { key: 'Sa\u011fArkaKapi', label: 'Sa\u011f Arka Kap\u0131' },
  { key: 'Sa\u011fArka\u00c7amurluk', label: 'Sa\u011f Arka \u00c7amurluk' },
];

const PANEL_OPTIONS = [
  { value: 'Orijinal', label: 'Orijinal' },
  { value: 'Boyali', label: 'Boyal\u0131' },
  { value: 'De\u011fi\u015fmi\u015f', label: 'De\u011fi\u015fmi\u015f' },
];

const buildInitialPanels = () => {
  const panels = {};
  PANEL_KEYS.forEach(({ key }) => { panels[key] = 'Orijinal'; });
  return panels;
};

const PricePredictor = () => {
  const [form, setForm] = useState({
    marka: '', seri: '', model: '', yil: 2020, kilometre: '',
    vitesTipi: 'D\u00fcz', yakitTipi: 'Benzin', kasaTipi: 'Sedan',
    renk: '', motorHacmi: '', motorGucu: '', cekis: '\u00d6nden\u00c7eki\u015f',
    aracDurumu: '\u0130kinciEl', ortalamaYakitTuketim: '', yakitDeposu: '',
    agirHasarKaydi: false, takasaUygun: false,
    kimden: 'Sahibinden',
    boyaliDegisen: buildInitialPanels(),
  });
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);
  const [showPanels, setShowPanels] = useState(false);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === 'checkbox' ? checked : value });
  };

  const handlePanelChange = (key, value) => {
    setForm({
      ...form,
      boyaliDegisen: { ...form.boyaliDegisen, [key]: value },
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setResult(null);
    try {
      // Backend PredictionController accepts a Car object
      const payload = {
        marka: form.marka,
        seri: form.seri,
        model: form.model,
        yil: Number(form.yil),
        fiyat: 0,
        kilometre: Number(form.kilometre),
        vitesTipi: form.vitesTipi,
        yakitTipi: form.yakitTipi,
        kasaTipi: form.kasaTipi,
        renk: form.renk || 'Beyaz',
        motorHacmi: Number(form.motorHacmi) || 0,
        motorGucu: Number(form.motorGucu) || 0,
        cekis: form.cekis,
        aracDurumu: form.aracDurumu,
        ortalamaYakitTuketim: Number(form.ortalamaYakitTuketim) || 0,
        yakitDeposu: Number(form.yakitDeposu) || 0,
        agirHasarKaydi: form.agirHasarKaydi,
        takasaUygun: form.takasaUygun,
        kimden: form.kimden,
        resimler: [],
        konum: '',
        boyaliDegisen: form.boyaliDegisen,
      };
      const res = await predictPrice(payload);
      setResult(res.data);
    } catch (err) {
      toast.error(err.response?.data?.hata || err.response?.data?.message || 'Tahmin yapilamadi');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="prediction-page">
      <div className="prediction-container">
        <div className="prediction-form-section">
          <h2><FaChartLine /> Fiyat Tahmini</h2>
          <p className="prediction-desc">Arac bilgilerini girerek yapay zeka ile tahmini fiyat ogren.</p>

          <form onSubmit={handleSubmit}>
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
                <input name="seri" value={form.seri} onChange={handleChange} required placeholder="orn: A3" />
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
                <label>Kilometre *</label>
                <input type="number" name="kilometre" value={form.kilometre} onChange={handleChange} required min={0} />
              </div>
              <div className="form-group">
                <label>Renk</label>
                <input name="renk" value={form.renk} onChange={handleChange} placeholder="orn: Beyaz" />
              </div>
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
                <label>Ort. Yakit Tuketimi (L/100km)</label>
                <input type="number" name="ortalamaYakitTuketim" value={form.ortalamaYakitTuketim} onChange={handleChange} min={0} step="0.1" />
              </div>
              <div className="form-group">
                <label>Yakit Deposu (L)</label>
                <input type="number" name="yakitDeposu" value={form.yakitDeposu} onChange={handleChange} min={0} />
              </div>
            </div>

            <div className="form-grid" style={{ marginTop: 12 }}>
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

            {/* Boya ve Degisen Panelleri */}
            <div className="panel-toggle" onClick={() => setShowPanels(!showPanels)}>
              {showPanels ? 'Boya/Degisen Panellerini Gizle' : 'Boya/Degisen Panellerini Goster'}
            </div>

            {showPanels && (
              <div className="panel-form-grid" style={{ marginTop: 12 }}>
                {PANEL_KEYS.map(({ key, label }) => (
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
            )}

            <button type="submit" className="btn btn-primary" disabled={loading} style={{ marginTop: 20 }}>
              {loading ? 'Tahmin yapiliyor...' : 'Fiyat Tahmin Et'}
            </button>
          </form>
        </div>

        {result && (
          <div className="prediction-result">
            <h3>Tahmini Fiyat</h3>
            <div className="prediction-price">
              {result.tahminSonucu?.fiyatEtiketi || 'Bilinmiyor'}
            </div>
            <p className="prediction-note">Bu tahmin yapay zeka modeli tarafindan yapilmistir. Gercek fiyatlar farklilik gosterebilir.</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default PricePredictor;
