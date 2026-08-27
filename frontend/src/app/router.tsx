import { Navigate, Route, Routes } from "react-router-dom";
import { AuthTest } from "../features/auth/AuthTest";
import { ProtectedRoute } from "../features/auth/ProtectedRoute";
import { PropertyCatalog } from "../features/properties/PropertyCatalog";
import { PropertyDetail } from "../features/properties/PropertyDetail";

export const AppRouter = () => (
  <Routes>
    <Route path="/" element={<Navigate to="/public" replace />} />
    <Route path="/public" element={<PropertyCatalog />} />
    <Route path="/properties/:propertyId" element={<PropertyDetail />} />
    <Route path="/private" element={<ProtectedRoute />} />
    <Route path="/auth-test" element={<AuthTest />} />
    <Route path="*" element={<Navigate to="/public" replace />} />
  </Routes>
);
