export type Pet = {
  id: string;
  name: string;
  species: string;
  breed: string;
  age: number;
  price: number;
  stock: number;
  createdAtUtc: string;
  updatedAtUtc: string;
};
export type SavePet = Omit<Pet, "id" | "createdAtUtc" | "updatedAtUtc">;
export type LoginResult = {
  token: string;
  username: string;
  role: string;
  expiresAtUtc: string;
};
