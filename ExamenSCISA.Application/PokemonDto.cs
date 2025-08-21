namespace ExamenSCISA.Application
{
    public class PokemonDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Especie { get; set; }
        public string ImagenUrl { get; set; }
        public int Altura { get; set; }
        public int Peso { get; set; }
        public List<string> Habilidades { get; set; } = new();
        public List<string> Tipos { get; set; } = new();
    }
}
