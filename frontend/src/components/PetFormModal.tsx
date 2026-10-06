import { useEffect, useState, type FormEvent } from "react";
import type { Pet, SavePet } from "../types/pet";

const blank: SavePet = {
  name: "",
  species: "",
  breed: "",
  age: 0,
  price: 0,
  stock: 0,
};

export function PetFormModal({
  pet,
  saving,
  onSave,
  onClose,
}: {
  pet?: Pet;
  saving: boolean;
  onSave: (value: SavePet) => Promise<void>;
  onClose: () => void;
}) {
  const [form, setForm] = useState<SavePet>(blank);
  const [error, setError] = useState("");

  useEffect(() => {
    setForm(
      pet
        ? {
            name: pet.name,
            species: pet.species,
            breed: pet.breed,
            age: pet.age,
            price: pet.price,
            stock: pet.stock,
          }
        : blank,
    );
    setError("");
  }, [pet]);

  const submit = async (event: FormEvent) => {
    event.preventDefault();
    setError("");
    try {
      await onSave(form);
      onClose();
    } catch (reason) {
      setError(
        reason instanceof Error ? reason.message : "Unable to save pet.",
      );
    }
  };
  const field = (
    key: keyof SavePet,
    label: string,
    type = "text",
    step?: string,
  ) => (
    <label>
      {label}
      <input
        required
        type={type}
        min={type === "number" ? 0 : undefined}
        step={step}
        value={form[key]}
        onChange={(event) =>
          setForm({
            ...form,
            [key]:
              type === "number"
                ? Number(event.target.value)
                : event.target.value,
          })
        }
      />
    </label>
  );

  return (
    <div className="modal-backdrop" role="presentation">
      <section
        className="pet-modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="pet-modal-title"
      >
        <div className="modal-header">
          <h2 id="pet-modal-title">{pet ? "Edit Pet" : "Add Pet"}</h2>
          <button
            type="button"
            className="close-button"
            aria-label="Close"
            onClick={onClose}
            disabled={saving}
          >
            ×
          </button>
        </div>
        <form onSubmit={submit}>
          <div className="form-grid">
            {field("name", "Name")}
            {field("species", "Species")}
            {field("breed", "Breed")}
            {field("age", "Age", "number")}
            {field("price", "Price", "number", "0.01")}
            {field("stock", "Stock", "number")}
          </div>
          {error && <p className="error">{error}</p>}
          <div className="actions modal-actions">
            <button type="button" onClick={onClose} disabled={saving}>
              Cancel
            </button>
            <button className="primary" disabled={saving}>
              {saving ? "Saving..." : pet ? "Save changes" : "Add pet"}
            </button>
          </div>
        </form>
      </section>
    </div>
  );
}
