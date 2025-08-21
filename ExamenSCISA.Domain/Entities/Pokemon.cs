namespace ExamenSCISA.Domain.Entities
{
    public class Pokemon
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public string Especie { get; private set; }
        public string ImagenUrl { get; private set; }

        public Pokemon(int id, string nombre, string especie, string imagenUrl)
        {
            try
            {
                Id = id;

                // validamos q el nombre no venga vacio
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new Exception("el nombre del pokemon no puede estar vacio");

                Nombre = nombre.Trim();

                // lo mismo con la especie
                if (string.IsNullOrWhiteSpace(especie))
                    throw new Exception("la especie del pokemon no puede estar vacia");

                Especie = especie.Trim();
                ImagenUrl = imagenUrl ?? "";
            }
            catch (Exception ex)
            {
                throw new Exception("error al crear el pokemon: " + ex.Message);
            }
        }
    }
}
