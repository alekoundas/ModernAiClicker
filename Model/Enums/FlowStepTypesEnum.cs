
namespace Model.Enums
{
    public enum TypesEnum
    {
        NO_SELECTION,
        TEMPLATE_SEARCH,
        MOUSE_MOVE_COORDINATES,
        MOUSE_CLICK,
        SLEEP,
        GO_TO,
        IS_NEW,     // Hidden. Not available for user selection.
        IS_SUCCESS, // Hidden. Not available for user selection.
        IS_FAILURE, // Hidden. Not available for user selection.
        WINDOW_RESIZE,
        WINDOW_MOVE,
        MOUSE_SCROLL,
        TEMPLATE_SEARCH_LOOP,
        MULTIPLE_TEMPLATE_SEARCH_LOOP,
        MULTIPLE_TEMPLATE_SEARCH,
        WAIT_FOR_TEMPLATE,
        LOOP,
        MULTIPLE_TEMPLATE_SEARCH_LOOP_CHILD, // Hidden. Not available for user selection.
        MULTIPLE_TEMPLATE_SEARCH_CHILD,      // Hidden. Not available for user selection.
    }
}
