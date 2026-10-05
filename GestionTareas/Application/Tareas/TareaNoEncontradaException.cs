namespace Application.Tareas
{
    public class TareaNoEncontradaException : Exception
    {
        public int TareaId { get; }

        public TareaNoEncontradaException(int tareaId)
            : base($"No se encontró la tarea con identificador {tareaId}.")
        {
            TareaId = tareaId;
        }
    }
}