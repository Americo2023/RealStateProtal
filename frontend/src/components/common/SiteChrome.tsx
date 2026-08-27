import { Link } from "react-router-dom";
import { apiUrl } from "../../services/apiClient";

export const SiteHeader = () => (
  <header className="site-header">
    <Link className="brand" to="/" aria-label="RealStatePortal inicio">
      <span className="brand-mark">R</span>
      <span>RealStatePortal</span>
    </Link>
    <nav className="main-nav" aria-label="Navegacion principal">
      <Link to="/public#properties">Propiedades</Link>
      <Link to="/private">Área privada</Link>
      <Link to="/favorites">Favoritos</Link>
      <Link to="/auth-test">Auth test</Link>
      <a href="#contact">Contacto</a>
    </nav>
    <a className="login-link" href={`${apiUrl}/auth/login`}>
      Iniciar sesion
    </a>
  </header>
);

export const Footer = () => (
  <footer id="contact">
    <span>RealStatePortal</span>
    <span>Propiedades que se sienten como hogar.</span>
  </footer>
);

export const PageMessage = ({
  title,
  message,
}: {
  title: string;
  message: string;
}) => (
  <div className="app-shell">
    <SiteHeader />
    <main className="message-page">
      <p className="eyebrow">RealStatePortal</p>
      <h1>{title}</h1>
      <p className="intro-copy">{message}</p>
    </main>
    <Footer />
  </div>
);
