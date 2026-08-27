import { BrowserRouter } from "react-router-dom";
import { AuthProvider } from "./AuthContext";
import { FavoritesProvider } from "./FavoritesContext";
import { AppRouter } from "./router";

export const App = () => (
  <BrowserRouter>
    <AuthProvider>
      <FavoritesProvider>
        <AppRouter />
      </FavoritesProvider>
    </AuthProvider>
  </BrowserRouter>
);
