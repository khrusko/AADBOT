import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import ProtectedRoute from "./components/ProtectedRoute";

import Navbar from "./components/Navbar";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import PhotoFeedPage from "./pages/PhotoFeedPage";
import PhotoDetailPage from "./pages/PhotoDetailPage";
import PhotoUploadPage from "./pages/PhotoUploadPage";
import EditPhotoPage from "./pages/EditPhotoPage";
import PackagePage from "./pages/PackagePage";
import AdminPage from "./pages/AdminPage";

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Navbar />
        <Routes>
          <Route path="/" element={<PhotoFeedPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/photo/:id" element={<PhotoDetailPage />} />
          <Route
            path="/package"
            element={
              <ProtectedRoute allowUser={true} allowAdmin={true}>
                <PackagePage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/upload"
            element={
              <ProtectedRoute allowUser={true} allowAdmin={true}>
                <PhotoUploadPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/photo/edit/:id"
            element={
              <ProtectedRoute allowUser={true} allowAdmin={true} >
                <EditPhotoPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin"
            element={
              <ProtectedRoute allowAdmin={true}>
                <AdminPage />
              </ProtectedRoute>
            }
          />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
