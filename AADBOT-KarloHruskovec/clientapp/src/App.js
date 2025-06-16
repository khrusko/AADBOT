import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import PhotoFeedPage from './pages/PhotoFeedPage';
import PhotoUploadPage from './pages/PhotoUploadPage';
import PhotoDetailPage from './pages/PhotoDetailPage';
import EditPhotoPage from './pages/EditPhotoPage';
import PackagePage from './pages/PackagePage';
import AdminPage from './pages/AdminPage';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<PhotoFeedPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/upload" element={<PhotoUploadPage />} />
        <Route path="/photo/:id" element={<PhotoDetailPage />} />
        <Route path="/photo/:id/edit" element={<EditPhotoPage />} />
        <Route path="/package" element={<PackagePage />} />
        <Route path="/admin" element={<AdminPage />} />
      </Routes>
    </Router>
  );
}

export default App;
