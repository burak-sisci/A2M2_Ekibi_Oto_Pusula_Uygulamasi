import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext.jsx';
import Navbar from './components/Layout/Navbar.jsx';
import Footer from './components/Layout/Footer.jsx';
import Login from './components/Auth/Login.jsx';
import Register from './components/Auth/Register.jsx';
import ListingList from './components/Listings/ListingList.jsx';
import ListingDetail from './components/Listings/ListingDetail.jsx';
import CreateListing from './components/Listings/CreateListing.jsx';
import FavoriteList from './components/Favorites/FavoriteList.jsx';
import PriceEstimator from './components/PriceEstimator/PriceEstimator.jsx';

function App() {
    return (
        <AuthProvider>
            <Router>
                <div className="app">
                    <Navbar />
                    <main className="main-content">
                        <Routes>
                            <Route path="/" element={<ListingList />} />
                            <Route path="/login" element={<Login />} />
                            <Route path="/register" element={<Register />} />
                            <Route path="/listings/:id" element={<ListingDetail />} />
                            <Route path="/create-listing" element={<CreateListing />} />
                            <Route path="/favorites" element={<FavoriteList />} />
                            <Route path="/price-estimator" element={<PriceEstimator />} />
                        </Routes>
                    </main>
                    <Footer />
                </div>
            </Router>
        </AuthProvider>
    );
}

export default App;
