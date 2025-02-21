
namespace Model.Enums
{
    public enum FlowStepTypesEnum
    {
        NO_SELECTION,
        WAIT,
        GO_TO,
        LOOP,

        FLOW,
        SUB_FLOW,

        WINDOW_MOVE,
        WINDOW_RESIZE,

        MOUSE_SCROLL,
        MOUSE_CLICK,
        MOUSE_MOVE_COORDINATES,

        TEMPLATE_SEARCH,
        TEMPLATE_SEARCH_LOOP,
        MULTIPLE_TEMPLATE_SEARCH_LOOP,
        MULTIPLE_TEMPLATE_SEARCH,
        WAIT_FOR_TEMPLATE,

        NEW,     // Hidden. Not available for user selection.
        SUCCESS, // Hidden. Not available for user selection.
        FAILURE, // Hidden. Not available for user selection.
        PARAMETERS, // Hidden. Not available for user selection.
        FLOW_STEPS, // Hidden. Not available for user selection.

        MULTIPLE_TEMPLATE_SEARCH_LOOP_CHILD, // Hidden. Not available for user selection.
        MULTIPLE_TEMPLATE_SEARCH_CHILD,      // Hidden. Not available for user selection.
    }
}
