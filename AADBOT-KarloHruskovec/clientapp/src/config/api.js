const BASE_URL = "https://localhost:44300";

const api = {
  base: BASE_URL,
  photo: {
    latest: `${BASE_URL}/api/photo/latest`,
    upload: `${BASE_URL}/api/photo/upload`,
    get: (id) => `${BASE_URL}/api/photo/${id}`,
    edit: (id) => `${BASE_URL}/api/photo/${id}`,
    search: `${BASE_URL}/api/photo/search`,
    download: (id) => `${BASE_URL}/api/photo/${id}/download`,
  },
  auth: {
    register: `${BASE_URL}/api/auth/register`,
    login: `${BASE_URL}/api/auth/login`,
    loginGoogle: `${BASE_URL}/api/auth/signin-google`,
    loginGitHub: `${BASE_URL}/api/auth/signin-github`,
    me: `${BASE_URL}/api/auth/me`,
    logout: `${BASE_URL}/api/auth/logout`,
  },
  package: {
    status: `${BASE_URL}/api/package/status`,
    change: `${BASE_URL}/api/package/change`,
  },
  admin: {
    users: `${BASE_URL}/api/admin/users`,
    logs: `${BASE_URL}/api/admin/logs`,
    photos: `${BASE_URL}/api/admin/photos`,
    changeUserPackage: (userId) =>
      `${BASE_URL}/api/admin/user/${userId}/package`,
    deletePhoto: (photoId) => `${BASE_URL}/api/admin/photo/${photoId}`,
  },
};

export default api;
