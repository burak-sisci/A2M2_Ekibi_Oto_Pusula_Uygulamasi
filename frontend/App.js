import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { AuthProvider } from './context/AuthContext';
import { ThemeProvider, useTheme } from './context/ThemeContext';
import Navbar from './components/Layout/Navbar';
import Footer from './components/Layout/Footer';
import ProtectedRoute from './components/ProtectedRoute';
import LoginForm from './components/Auth/LoginForm';
import RegisterForm from './components/Auth/RegisterForm';
import ForgotPassword from './components/Auth/ForgotPassword';
import ResetPassword from './components/Auth/ResetPassword';
import Profile from './components/Auth/Profile';
import HomePage from './components/Home/HomePage';
import CarList from './components/Cars/CarList';
import CarDetail from './components/Cars/CarDetail';
import CarForm from './components/Cars/CarForm';
import UserLists from './components/Lists/UserLists';
import PricePredictor from './components/Prediction/PricePredictor';
import './App.css';

function AppContent() {
  const { dark } = useTheme();

  return (
    <Router>
      <div className="app">
        <Navbar />
        <main className="main-content">
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/cars" element={<CarList />} />
            <Route path="/login" element={<LoginForm />} />
            <Route path="/register" element={<RegisterForm />} />
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route path="/reset-password" element={<ResetPassword />} />
            <Route path="/cars/:id" element={<CarDetail />} />
            <Route path="/prediction" element={<PricePredictor />} />
            <Route path="/cars/new" element={
              <ProtectedRoute><CarForm /></ProtectedRoute>
            } />
            <Route path="/cars/:id/edit" element={
              <ProtectedRoute><CarForm /></ProtectedRoute>
            } />
            <Route path="/profile" element={
              <ProtectedRoute><Profile /></ProtectedRoute>
            } />
            <Route path="/lists" element={
              <ProtectedRoute><UserLists /></ProtectedRoute>
            } />
          </Routes>
        </main>
        <Footer />
      </div>
      <ToastContainer
        position="top-right"
        autoClose={3000}
        theme={dark ? 'dark' : 'light'}
        hideProgressBar={false}
        closeOnClick
      />
    </Router>
  );
}

function App() {
  return (
    <AuthProvider>
      <ThemeProvider>
        <AppContent />
      </ThemeProvider>
    </AuthProvider>
  );
}

export default App;
