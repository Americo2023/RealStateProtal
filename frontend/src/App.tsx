import { BrowserRouter, Link, Route, Routes } from 'react-router-dom'
import './App.css'


function HomePage() {
  return (
    <main>
      <h1>RealStatePortal</h1>
      <p>El catálogo de propiedades estará disponible en la siguiente fase.</p>
      <Link to="/properties">Explorar propiedades</Link>
    </main>
  )
}

function PropertiesPage() {
  return (
    <main>
      <h1>Propiedades</h1>
      <p>La búsqueda y el catálogo se implementarán en la fase de frontend.</p>
      <Link to="/">Volver al inicio</Link>
    </main>
  )
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/properties" element={<PropertiesPage />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
