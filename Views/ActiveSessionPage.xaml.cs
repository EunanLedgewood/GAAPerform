using GAAPerform.Models;
using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class ActiveSessionPage : ContentPage
{
    private readonly ActiveSessionViewModel _vm;
    private bool _pickerInitialising = false;

    public ActiveSessionPage(ActiveSessionViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    public void LoadSession(TrainingDay day, SessionDetail detail)
    {
        _vm.LoadSession(day, detail);
    }

    private void OnFinishTapped(object? sender, EventArgs e)
    {
        _vm.FinishSession();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.Cleanup();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.IsFinished)
            await Navigation.PopAsync();
    }

    // Auto-fill weight when entry loses focus
    public void OnWeightCompleted(object? sender, EventArgs e)
    {
        if (sender is Entry entry && entry.BindingContext is ExerciseSet set)
        {
            var exercise = FindExerciseForSet(set);
            if (exercise != null)
            {
                var index = exercise.Sets.IndexOf(set);
                _vm.OnWeightChanged(exercise, index, set.Weight);
            }
        }
    }

    public void OnRepsCompleted(object? sender, EventArgs e)
    {
        if (sender is Entry entry && entry.BindingContext is ExerciseSet set)
        {
            var exercise = FindExerciseForSet(set);
            if (exercise != null)
            {
                var index = exercise.Sets.IndexOf(set);
                _vm.OnRepsChanged(exercise, index, set.Reps);
            }
        }
    }

    public void OnTimeCompleted(object? sender, EventArgs e)
    {
        if (sender is Entry entry && entry.BindingContext is ExerciseSet set)
        {
            var exercise = FindExerciseForSet(set);
            if (exercise != null)
            {
                var index = exercise.Sets.IndexOf(set);
                _vm.OnTimeChanged(exercise, index, set.Time);
            }
        }
    }

    public void OnDifficultyCompleted(object? sender, EventArgs e)
    {
        if (sender is Entry entry && entry.BindingContext is ExerciseSet set)
        {
            var exercise = FindExerciseForSet(set);
            if (exercise != null)
            {
                var index = exercise.Sets.IndexOf(set);
                _vm.OnDifficultyChanged(exercise, index, set.Difficulty);
            }
        }
    }

    private CompletedExercise? FindExerciseForSet(ExerciseSet set)
    {
        return _vm.CurrentExercises.FirstOrDefault(e => e.Sets.Contains(set));
    }

    private void OnFieldTypeSelected(object? sender, EventArgs e)
    {
        if (_pickerInitialising) return;
        if (sender is not Picker picker) return;
        if (picker.BindingContext is not CompletedExercise exercise) return;

        var newType = picker.SelectedIndex switch
        {
            0 => ExerciseFieldType.WeightsAndReps,
            1 => ExerciseFieldType.TimeAndDifficulty,
            2 => ExerciseFieldType.RepsOnly,
            3 => ExerciseFieldType.Custom,
            _ => ExerciseFieldType.WeightsAndReps
        };

        exercise.FieldType = newType;
        _vm.RefreshExercise(exercise);
    }

    private void OnPickerLoaded(object? sender, EventArgs e)
    {
        if (sender is not Picker picker) return;
        if (picker.BindingContext is not CompletedExercise exercise) return;

        _pickerInitialising = true;
        picker.SelectedIndex = exercise.FieldType switch
        {
            ExerciseFieldType.WeightsAndReps => 0,
            ExerciseFieldType.TimeAndDifficulty => 1,
            ExerciseFieldType.RepsOnly => 2,
            ExerciseFieldType.Custom => 3,
            _ => 0
        };
        _pickerInitialising = false;
    }
}