export default function HomePage() {
  return (
    <div className="max-w-2xl mx-auto text-center py-12">
      <h2 className="font-serif text-3xl font-semibold text-parchment-900 dark:text-white">
        Welcome to The Temple Courts
      </h2>
      <p className="mt-4 text-lg text-parchment-600 dark:text-slate-400">
        A place for Bible study and community — guided by questions, rooted in Scripture.
      </p>
      <blockquote className="mt-8 mx-auto max-w-md font-serif italic text-parchment-500 dark:text-slate-500 text-sm leading-relaxed border-l-2 border-parchment-200 dark:border-slate-700 pl-4">
        After three days they found him in the temple courts, sitting among the teachers, listening to them and asking them questions.
        <cite className="block mt-1 not-italic text-xs text-parchment-400 dark:text-slate-600">Luke 2:46</cite>
      </blockquote>
    </div>
  );
}
