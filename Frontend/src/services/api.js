import axios from "axios";

const api = axios.create({
    baseURL: "/api",
    timeout: 15000,
});

api.interceptors.request.use(
    (config) => {

        const url = config.url || "";

        const isAdminApi =
            url.toLowerCase().startsWith("/adminauth") ||
            url.toLowerCase().startsWith("/admin-dashboard") ||
            url.toLowerCase().startsWith("/admin");

        const token = isAdminApi
            ? localStorage.getItem("admin_jwt_token")
            : localStorage.getItem("jwt_token");

        if (token) {
            config.headers = config.headers || {};
            config.headers.Authorization = `Bearer ${token}`;
        }

        return config;
    },
    (error) => Promise.reject(error)
);
api.interceptors.response.use(
    (response) => response,

    (error) => {

        if (error?.response?.status === 401) {

            const url = error?.config?.url || "";

            const isAdminApi =
                url.toLowerCase().startsWith("/adminauth") ||
                url.toLowerCase().startsWith("/admin-dashboard") ||
                url.toLowerCase().startsWith("/admin");

            if (isAdminApi) {

                localStorage.removeItem("admin_jwt_token");
                localStorage.removeItem("admin_profile");

                if (
                    window.location.pathname.startsWith("/admin") &&
                    window.location.pathname !== "/admin"
                ) {
                    window.location.replace("/admin");
                }
            }
        }

        return Promise.reject(error);
    }
);

export default api;