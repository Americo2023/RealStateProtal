import { useState } from 'react'
import './App.css'

interface Property {
  id: number
  title: string
  location: string
  price: string
  details: string
  image: string
}

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

function App() {
  const [search, setSearch] = useState('')
  const normalizedSearch = search.trim().toLowerCase()
  const visibleProperties = properties.filter((property) =>
    `${property.title} ${property.location}`.toLowerCase().includes(normalizedSearch),
  )

  return (
    <div className="app-shell">
      <header className="site-header">
        <a className="brand" href="/" aria-label="RealStatePortal inicio">
          <span className="brand-mark">R</span>
          <span>RealStatePortal</span>
        </a>
        <nav className="main-nav" aria-label="Navegacion principal">
          <a className="active" href="#properties">Propiedades</a>
          <a href="#about">Como funciona</a>
          <a href="#contact">Contacto</a>
        </nav>
        <a className="login-link" href="http://localhost:5080/auth/login">Iniciar sesion</a>
      </header>

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

      <footer id="contact">
        <span>RealStatePortal</span>
        <span>Propiedades que se sienten como hogar.</span>
      </footer>
    </div>
  )
}

export default App
