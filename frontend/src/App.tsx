import { useEffect, useState } from 'react'
import './App.css'

interface Property {
  id: number
  title: string
  location: string
  price: string
  details: string
  image: string
}

interface AuthUser {
  isAuthenticated: boolean
  userName: string | null
  email: string | null
  auth0UserId: string | null
  roles: string[]
}

const apiUrl = 'http://localhost:5080'

const properties: Property[] = [
  {
    id: 1,
    title: 'Apartamento luminoso',
    location: 'Valencia, Ruzafa',
    price: '285.000 EUR',
    details: '3 habitaciones · 2 banos · 104 m2',
    image: 'https://images.unsplash.com/photo-1600607687920-4e2a09cf159d?auto=format&fit=crop&w=900&q=80',
  },
  {
    id: 2,
    title: 'Casa con jardin',
    location: 'Alicante, San Juan',
    price: '420.000 EUR',
    details: '4 habitaciones · 3 banos · 188 m2',
    image: 'https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&w=900&q=80',
  },
  {
    id: 3,
    title: 'Atico con terraza',
    location: 'Madrid, Chamberi',
    price: '610.000 EUR',
    details: '2 habitaciones · 2 banos · 92 m2',
    image: 'https://images.unsplash.com/photo-1600607688969-a5bfcd646154?auto=format&fit=crop&w=900&q=80',
  },
]

const App = () => window.location.pathname === '/auth-test' ? <AuthTest /> : <Home />

const Home = () => {
  const [search, setSearch] = useState('')
  const normalizedSearch = search.trim().toLowerCase()
  const visibleProperties = properties.filter((property) =>
    `${property.title} ${property.location}`.toLowerCase().includes(normalizedSearch),
  )

  return (
    <div className="app-shell">
      <SiteHeader />
      <main>
        <section className="intro" id="about">
          <div>
            <p className="eyebrow">Encuentra tu proximo lugar</p>
            <h1>Espacios para vivir bien.</h1>
            <p className="intro-copy">Explora propiedades seleccionadas y encuentra un hogar que encaje contigo.</p>
          </div>
          <div className="intro-note">
            <strong>Catalogo abierto</strong>
            <span>Propiedades verificadas por nuestro equipo.</span>
          </div>
        </section>

        <section className="search-panel" aria-label="Buscar propiedades">
          <label htmlFor="property-search">Buscar por ciudad o zona</label>
          <div className="search-row">
            <input
              id="property-search"
              type="search"
              placeholder="Ej. Valencia, Chamberi..."
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
            <button type="button" onClick={() => setSearch('')}>Limpiar</button>
          </div>
        </section>

        <section className="property-section" id="properties">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Seleccion actual</p>
              <h2>Propiedades destacadas</h2>
            </div>
            <span className="result-count">{visibleProperties.length} resultados</span>
          </div>
          <div className="property-grid">
            {visibleProperties.map((property) => (
              <article className="property-card" key={property.id}>
                <img src={property.image} alt={property.title} />
                <div className="property-content">
                  <p className="property-location">{property.location}</p>
                  <h3>{property.title}</h3>
                  <p className="property-details">{property.details}</p>
                  <strong className="property-price">{property.price}</strong>
                </div>
              </article>
            ))}
          </div>
          {visibleProperties.length === 0 && <p className="empty-state">No encontramos propiedades para esa busqueda.</p>}
        </section>
      </main>
      <Footer />
    </div>
  )
}

const AuthTest = () => {
  const [user, setUser] = useState<AuthUser | null>(null)
  const [publicResult, setPublicResult] = useState<string>('Sin probar')
  const [protectedResult, setProtectedResult] = useState<string>('Sin probar')
  const [error, setError] = useState<string | null>(null)

  const loadUser = async () => {
    try {
      setError(null)
      const response = await fetch(`${apiUrl}/api/auth/me`, { credentials: 'include' })
      if (!response.ok) {
        setUser(null)
        return
      }
      setUser(await response.json() as AuthUser)
    } catch {
      setError('No se pudo conectar con la API.')
    }
  }

  const testEndpoint = async (endpoint: string, setResult: (result: string) => void) => {
    try {
      const response = await fetch(`${apiUrl}${endpoint}`, { credentials: 'include' })
      setResult(`${response.status} ${response.statusText}`)
    } catch {
      setResult('Error de conexión')
    }
  }

  useEffect(() => {
    void Promise.resolve().then(loadUser)
  }, [])

  return (
    <div className="app-shell auth-test-page">
      <SiteHeader />
      <main>
        <section className="auth-test-intro">
          <p className="eyebrow">Fase 8 · Auth0</p>
          <h1>Prueba de autenticación</h1>
          <p className="intro-copy">Comprueba la sesión del portal y el acceso a sus endpoints.</p>
        </section>
        <section className="auth-actions" aria-label="Acciones de autenticación">
          <a className="primary-action" href={`${apiUrl}/auth/login`}>Iniciar sesión</a>
          <a className="secondary-action" href={`${apiUrl}/auth/logout`}>Cerrar sesión</a>
          <button type="button" onClick={() => void loadUser()}>Actualizar sesión</button>
        </section>
        {error && <p className="error-message">{error}</p>}
        <section className="auth-grid">
          <div className="auth-panel">
            <div className="panel-heading">
              <p className="eyebrow">Identidad</p>
              <span className={user?.isAuthenticated ? 'status active-status' : 'status'}>
                {user?.isAuthenticated ? 'Autenticado' : 'No autenticado'}
              </span>
            </div>
            <dl className="identity-list">
              <div><dt>Nombre de usuario</dt><dd>{user?.userName ?? 'No disponible'}</dd></div>
              <div><dt>Email</dt><dd>{user?.email ?? 'No disponible'}</dd></div>
              <div><dt>Auth0 User Id</dt><dd>{user?.auth0UserId ?? 'No disponible'}</dd></div>
              <div><dt>Roles</dt><dd>{user?.roles.join(', ') || 'Ninguno'}</dd></div>
            </dl>
          </div>
          <div className="auth-panel endpoint-panel">
            <p className="eyebrow">Endpoints</p>
            <div className="endpoint-row">
              <span>Catálogo público</span>
              <strong>{publicResult}</strong>
              <button type="button" onClick={() => void testEndpoint('/api/properties', setPublicResult)}>Probar</button>
            </div>
            <div className="endpoint-row">
              <span>Perfil protegido</span>
              <strong>{protectedResult}</strong>
              <button type="button" onClick={() => void testEndpoint('/api/auth/me', setProtectedResult)}>Probar</button>
            </div>
          </div>
        </section>
      </main>
      <Footer />
    </div>
  )
}

const SiteHeader = () => (
  <header className="site-header">
    <a className="brand" href="/" aria-label="RealStatePortal inicio">
      <span className="brand-mark">R</span>
      <span>RealStatePortal</span>
    </a>
    <nav className="main-nav" aria-label="Navegacion principal">
      <a href="/#properties">Propiedades</a>
      <a href="/auth-test">Auth test</a>
      <a href="/#contact">Contacto</a>
    </nav>
    <a className="login-link" href={`${apiUrl}/auth/login`}>Iniciar sesion</a>
  </header>
)

const Footer = () => (
  <footer id="contact">
    <span>RealStatePortal</span>
    <span>Propiedades que se sienten como hogar.</span>
  </footer>
)

export default App
