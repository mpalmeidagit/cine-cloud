namespace BuildingBlocks.Core.ValidationMessages;

public class ValidationMessages
{
    public const string MIN_LENGTH_ERROR_MESSAGE = "{PropertyName} deve ter no mínimo {MinLength} caracteres";
    public const string MAX_LENGTH_ERROR_MESSAGE = "{PropertyName} não deve ultrapassar {MaxLength} caracteres";
    public const string EMPTY_STRING_ERROR_MESSAGE = "{PropertyName} não pode ser vazio";
    public const string ERROR_MESSAGE = "{PropertyName} inválido";
}