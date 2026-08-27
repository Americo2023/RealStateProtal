import { useEffect, useState, type FormEvent } from "react";
import { Footer, PageMessage, SiteHeader } from "../../components/common/SiteChrome";
import { adminApi } from "../../services/adminApi";
import type { AdminUser } from "../../types/AdminUser";
import type { AuditLog } from "../../types/AuditLog";
import type { Broker } from "../../types/Broker";
import type { PropertyCard } from "../../types/PropertyCard";

const roles = ["Visitor", "Registered User", "Broker", "Administrator"];

type AdminSection = "users" | "brokers" | "properties" | "audit";

export const AdminPortal = () => {
  const [section, setSection] = useState<AdminSection>("users");
  const [users, setUsers] = useState<AdminUser[]>([]);
  const [brokers, setBrokers] = useState<Broker[]>([]);
  const [properties, setProperties] = useState<PropertyCard[]>([]);
  const [logs, setLogs] = useState<AuditLog[]>([]);
  const [editingBroker, setEditingBroker] = useState<Broker | null>(null);
  const [isSavingBroker, setIsSavingBroker] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      try {
        const [loadedUsers, loadedBrokers, loadedProperties, loadedLogs] =
          await Promise.all([
            adminApi.getUsers(),
            adminApi.getBrokers(),
            adminApi.getProperties(),
            adminApi.getAuditLogs(),
          ]);
        setUsers(loadedUsers);
        setBrokers(loadedBrokers);
        setProperties(
          loadedProperties.map((property) => ({
            id: property.id,
            title: property.title,
            location: property.address?.city ?? "Sin dirección",
            price: `${property.price.toLocaleString("es-ES")} ${property.currency}`,
            details: `${property.bedrooms} habitaciones · ${property.bathrooms} baños`,
            image: "",
            status: property.status,
          })),
        );
        setLogs(loadedLogs);
      } catch {
        setError("No se pudieron cargar los datos administrativos.");
      } finally {
        setLoading(false);
      }
    };
    void load();
  }, []);

  const updateUser = async (user: AdminUser, isActive: boolean, role: string) => {
    try {
      const updated = await adminApi.updateUser(user.id, {
        isActive,
        roles: [role],
      });
      setUsers((current) =>
        current.map((candidate) => (candidate.id === updated.id ? updated : candidate)),
      );
    } catch {
      setError("No se pudo actualizar el usuario.");
    }
  };

  const updateBroker = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!editingBroker) {
      return;
    }

    setIsSavingBroker(true);
    try {
      const updated = await adminApi.updateBroker(editingBroker.id, {
        fullName: editingBroker.fullName,
        email: editingBroker.email,
        phone: editingBroker.phone,
        bio: editingBroker.bio,
        isActive: editingBroker.isActive,
      });
      setBrokers((current) =>
        current.map((broker) => (broker.id === updated.id ? updated : broker)),
      );
      setEditingBroker(null);
    } catch {
      setError("No se pudo actualizar el broker.");
    } finally {
      setIsSavingBroker(false);
    }
  };

  return (
    <div className="app-shell">
      <SiteHeader />
      <main className="admin-page">
        <section className="area-heading">
          <div>
            <p className="eyebrow">Administración</p>
            <h1>Control del portal.</h1>
            <p className="intro-copy">Gestiona usuarios, brokers, propiedades y auditoría.</p>
          </div>
        </section>
        <nav className="admin-tabs" aria-label="Secciones de administración">
          {(["users", "brokers", "properties", "audit"] as AdminSection[]).map((item) => (
            <button
              className={section === item ? "active" : ""}
              key={item}
              type="button"
              onClick={() => setSection(item)}
            >
              {{ users: "Usuarios", brokers: "Brokers", properties: "Propiedades", audit: "Auditoría" }[item]}
            </button>
          ))}
        </nav>
        {loading && <p className="empty-state">Cargando administración...</p>}
        {error && <p className="error-message">{error}</p>}
        {!loading && section === "users" && (
          <section className="admin-list">
            {users.map((user) => (
              <article className="admin-row" key={user.id}>
                <div><strong>{user.firstName} {user.lastName}</strong><span>{user.email}</span></div>
                <select value={user.roles[0] ?? "Visitor"} onChange={(event) => void updateUser(user, user.isActive, event.target.value)}>{roles.map((role) => <option key={role}>{role}</option>)}</select>
                <label className="toggle-label"><input type="checkbox" checked={user.isActive} onChange={(event) => void updateUser(user, event.target.checked, user.roles[0] ?? "Visitor")} /> Activo</label>
              </article>
            ))}
          </section>
        )}
        {!loading && section === "brokers" && (
          <section className="admin-list">
            {brokers.map((broker) => (
              <article className="admin-row" key={broker.id}>
                <div>
                  <strong>{broker.fullName}</strong>
                  <span>{broker.email} · {broker.phone}</span>
                </div>
                <span className="status">
                  {broker.isActive ? "Activo" : "Inactivo"}
                </span>
                <button type="button" onClick={() => setEditingBroker(broker)}>
                  Editar
                </button>
              </article>
            ))}
          </section>
        )}
        {editingBroker && (
          <div className="modal-backdrop" role="presentation">
            <form className="property-modal admin-modal" onSubmit={updateBroker}>
              <div className="modal-heading">
                <div>
                  <p className="eyebrow">Perfil de broker</p>
                  <h2>Editar información</h2>
                </div>
                <button type="button" onClick={() => setEditingBroker(null)}>
                  Cerrar
                </button>
              </div>
              <label>
                Nombre completo
                <input
                  required
                  value={editingBroker.fullName}
                  onChange={(event) =>
                    setEditingBroker({
                      ...editingBroker,
                      fullName: event.target.value,
                    })
                  }
                />
              </label>
              <label>
                Email
                <input
                  required
                  type="email"
                  value={editingBroker.email}
                  onChange={(event) =>
                    setEditingBroker({ ...editingBroker, email: event.target.value })
                  }
                />
              </label>
              <label>
                Teléfono
                <input
                  required
                  value={editingBroker.phone}
                  onChange={(event) =>
                    setEditingBroker({ ...editingBroker, phone: event.target.value })
                  }
                />
              </label>
              <label>
                Biografía
                <textarea
                  required
                  value={editingBroker.bio}
                  onChange={(event) =>
                    setEditingBroker({ ...editingBroker, bio: event.target.value })
                  }
                />
              </label>
              <label className="toggle-label">
                <input
                  type="checkbox"
                  checked={editingBroker.isActive}
                  onChange={(event) =>
                    setEditingBroker({
                      ...editingBroker,
                      isActive: event.target.checked,
                    })
                  }
                />
                Activo
              </label>
              <div className="modal-actions">
                <button type="button" onClick={() => setEditingBroker(null)}>
                  Cancelar
                </button>
                <button disabled={isSavingBroker} type="submit">
                  {isSavingBroker ? "Guardando..." : "Guardar cambios"}
                </button>
              </div>
            </form>
          </div>
        )}
        {!loading && section === "properties" && <section className="admin-list">{properties.map((property) => <article className="admin-row" key={property.id}><div><strong>{property.title}</strong><span>{property.location} · {property.price}</span></div><span className="status">{property.status}</span></article>)}</section>}
        {!loading && section === "audit" && <section className="admin-list">{logs.map((log) => <article className="admin-row" key={log.id}><div><strong>{log.action} · {log.entityName}</strong><span>{log.details}</span></div><time dateTime={log.changedAt}>{new Date(log.changedAt).toLocaleString("es-ES")}</time></article>)}</section>}
      </main>
      <Footer />
    </div>
  );
};

export const AdminAccessMessage = ({ isAdministrator }: { isAdministrator: boolean }) =>
  isAdministrator ? <AdminPortal /> : <PageMessage title="Acceso restringido" message="Esta sección está reservada para administradores." />;
