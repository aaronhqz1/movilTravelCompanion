namespace movilTravelCompanion;

// Cada vez que se navega a Dashboard/TravelDestination/Preferencias por push
// (desde el Flyout, o al confirmar un destino nuevo), esto saca del
// NavigationStack las paginas intermedias, dejando solo la raiz del Shell
// (HomePage) y la pagina recien alcanzada. Sin esto, cada viaje entre esas
// pantallas por el menu apila paginas nuevas sin limite: navegar
// Dashboard -> Cambiar Destino -> confirmar, repetido, hace crecer la pila
// indefinidamente en vez de quedarse en un tamaño estable.
internal static class ShellNavigationHelper
{
    public static void TrimNavigationStack()
    {
        var navigation = Shell.Current.Navigation;
        var stack = navigation.NavigationStack;

        // stack[0] es la raiz (HomePage), stack[^1] es la pagina a la que se
        // acaba de navegar; se remueven todas las que quedaron en el medio.
        for (var i = stack.Count - 2; i >= 1; i--)
        {
            navigation.RemovePage(stack[i]);
        }
    }
}
