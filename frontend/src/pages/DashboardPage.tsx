import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { PetFormModal } from "../components/PetFormModal";
import { createPet, deletePet, getPets, updatePet } from "../services/api";
import type { Pet, SavePet } from "../types/pet";
export function DashboardPage({
  token,
  username,
  onLogout,
}: {
  token: string;
  username: string;
  onLogout: () => void;
}) {
  const navigate = useNavigate();
  const [pets, setPets] = useState<Pet[]>([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [editing, setEditing] = useState<Pet | undefined>();
  const [showForm, setShowForm] = useState(false);
  const load = async () => {
    setLoading(true);
    setError("");
    try {
      setPets(await getPets(token));
    } catch (reason) {
      setError(
        reason instanceof Error ? reason.message : "Unable to load pets.",
      );
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => {
    void load();
  }, [token]);
  const closeModal = () => {
    setShowForm(false);
    setEditing(undefined);
  };
  const save = async (value: SavePet) => {
    setSaving(true);
    setError("");
    try {
      if (editing) await updatePet(editing.id, value, token);
      else await createPet(value, token);
      await load();
    } catch (reason) {
      setError(
        reason instanceof Error ? reason.message : "Unable to save pet.",
      );
      throw reason;
    } finally {
      setSaving(false);
    }
  };
  const remove = async (pet: Pet) => {
    if (!window.confirm(`Delete ${pet.name}?`)) return;
    try {
      await deletePet(pet.id, token);
      await load();
    } catch (reason) {
      setError(
        reason instanceof Error ? reason.message : "Unable to delete pet.",
      );
    }
  };
  const logout = () => {
    onLogout();
    navigate("/login", { replace: true });
  };
  return (
    <main className="dashboard">
      <header>
        <div>
          <p className="eyebrow">Pet Shop Management</p>
          <h1>Inventory dashboard</h1>
          <p className="muted">Signed in as {username}</p>
        </div>
        <div className="actions">
          <button onClick={() => void load()}>Refresh</button>
          <button onClick={logout}>Log out</button>
        </div>
      </header>
      {error && <p className="error">{error}</p>}
      <section className="toolbar">
        <h2>Pets ({pets.length})</h2>
        <button
          className="primary"
          onClick={() => {
            setEditing(undefined);
            setShowForm(true);
          }}
        >
          Add pet
        </button>
      </section>
      {showForm && (
        <PetFormModal
          pet={editing}
          saving={saving}
          onSave={save}
          onClose={closeModal}
        />
      )}{" "}
      {loading ? (
        <p className="status">Loading inventory...</p>
      ) : pets.length === 0 ? (
        <p className="status">No pets are currently in inventory.</p>
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Species</th>
                <th>Breed</th>
                <th>Age</th>
                <th>Price</th>
                <th>Stock</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {pets.map((pet) => (
                <tr key={pet.id}>
                  <td>{pet.name}</td>
                  <td>{pet.species}</td>
                  <td>{pet.breed}</td>
                  <td>{pet.age}</td>
                  <td>${pet.price.toFixed(2)}</td>
                  <td>{pet.stock}</td>
                  <td className="row-actions">
                    <button
                      onClick={() => {
                        setEditing(pet);
                        setShowForm(true);
                      }}
                    >
                      Edit
                    </button>
                    <button className="danger" onClick={() => void remove(pet)}>
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </main>
  );
}
